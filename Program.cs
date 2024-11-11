using Microsoft.AspNetCore.Identity;
using YogeshFurnitureAPI;
using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Seeders;
using YogeshFurnitureAPI.Model.Account;
using Microsoft.EntityFrameworkCore;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Service;
using YogeshFurnitureAPI.Helper.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<YogeshFurnitureDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<YogeshFurnitureUsers, IdentityRole>()
    .AddEntityFrameworkStores<YogeshFurnitureDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<JWTService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Add controllers and Swagger documentation
builder.Services.AddControllers();
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
    var userManager = services.GetRequiredService<UserManager<YogeshFurnitureUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<AdminUserInitializer>>();

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
app.UseAuthorization();

app.MapControllers();
app.Run();
