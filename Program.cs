using Microsoft.AspNetCore.Identity;
using YogeshFurnitureAPI;
using YogeshFurnitureAPI.Data;
using YogeshFurnitureAPI.Seeders;
using YogeshFurnitureAPI.Model.Account;
using Microsoft.EntityFrameworkCore;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext for connection to the database
builder.Services.AddDbContext<YogeshFurnitureDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services for user authentication and authorization
builder.Services.AddIdentity<YogeshFurnitureUsers, IdentityRole>()
    .AddEntityFrameworkStores<YogeshFurnitureDbContext>()
    .AddDefaultTokenProviders();

// Add other services to the container
builder.Services.AddScoped<IAccountService, AccountService>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<YogeshFurnitureUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var logger = services.GetRequiredService<ILogger<AdminUserInitializer>>();

    await AdminUserInitializer.Initialize(services, userManager, roleManager, logger);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Yogesh Furniture API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
