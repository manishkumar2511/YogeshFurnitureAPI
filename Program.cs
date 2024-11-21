using Microsoft.AspNetCore.Identity;
using YogeshFurnitureAPI;
using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Seeders;
using YogeshFurnitureAPI.Model.Account;
using Microsoft.EntityFrameworkCore;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Service;
using YogeshFurnitureAPI.Helper.Services;
using YogeshFurnitureAPI.Interface;
using YogeshFurnitureAPI.Interface.Notification;
using YogeshFurnitureAPI.Service.Notification;

var builder = WebApplication.CreateBuilder(args);

// Load UserSecrets for development
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Configure services
builder.Services.AddDbContext<YogeshFurnitureDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<YogeshFurnitureUsers, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // Require at least one digit
    options.Password.RequireLowercase = false; // Require at least one lowercase letter
    options.Password.RequireUppercase = false; // Require at least one uppercase letter
    options.Password.RequireNonAlphanumeric = false; // Require at least one special character
    options.Password.RequiredLength = 3; // Set minimum password length (e.g., 12 characters)
    options.Password.RequiredUniqueChars = 0; // Set the number of unique characters required
})
    .AddEntityFrameworkStores<YogeshFurnitureDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add controllers and Swagger documentation
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Create scope for role and user initialization
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var userManager = services.GetRequiredService<UserManager<YogeshFurnitureUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<AdminUserInitializer>>();

    // Check for required admin credentials in configuration
    var adminEmail = configuration["Admin:Email"];
    var adminPassword = configuration["Admin:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
    {
        logger.LogError("Admin email or password is not configured in user secrets.");
        throw new Exception("Admin email or password is not configured in user secrets.");
    }

    // Initialize roles and admin user
    await AdminUserInitializer.Initialize(services, userManager, roleManager, logger);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Yogesh Furniture API V1");
    });
}

// Enable CORS policy
app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();
app.Run();
