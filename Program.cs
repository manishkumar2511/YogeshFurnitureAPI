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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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


// Configure Authentication with JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Key"))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\":\"Authentication failed. Token is invalid or expired.\"}");
            },
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            }
        };
    });


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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null; 
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
app.UseMiddleware<AuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
