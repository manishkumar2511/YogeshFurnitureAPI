using Microsoft.AspNetCore.Identity;
using YogeshFurnitureAPI.Model.Account;
using System.Security.Claims;

namespace YogeshFurnitureAPI.Seeders
{
    public class AdminUserInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<YogeshFurnitureUsers> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminUserInitializer> logger)
        {
            // Check if the "Admin" role exists, if not, create it
            var role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                role = new IdentityRole("Admin");
                var roleResult = await roleManager.CreateAsync(role);
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Admin role created successfully.");
                }
                else
                {
                    logger.LogError("Error creating Admin role: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }

            var user = await userManager.FindByEmailAsync("testuser@yopmail.com");

            if (user == null)
            {
                user = new YogeshFurnitureUsers
                {
                    UserName = "testuser@yopmail.com",
                    Email = "testuser@yopmail.com",
                    PhoneNumber = "9117993206",
                    CreatedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(user, "Admin");

                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Admin user added to the Admin role.");
                    }
                    else
                    {
                        logger.LogError("Error adding user to Admin role: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }

                    var emailClaimResult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                    var phoneClaimResult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                    var roleClaimResult = await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Admin"));

                    if (emailClaimResult.Succeeded && phoneClaimResult.Succeeded && roleClaimResult.Succeeded)
                    {
                        logger.LogInformation("Admin user created with claims: Email = {Email}, Phone = {PhoneNumber}, Role = Admin", user.Email, user.PhoneNumber);
                    }
                    else
                    {
                        logger.LogError("Failed to add claims to admin user: Email Claim - {EmailError}, Phone Claim - {PhoneError}, Role Claim - {RoleError}",
                                         string.Join(", ", emailClaimResult.Errors.Select(e => e.Description)),
                                         string.Join(", ", phoneClaimResult.Errors.Select(e => e.Description)),
                                         string.Join(", ", roleClaimResult.Errors.Select(e => e.Description)));
                    }

                    logger.LogInformation("Admin user created successfully with email: {Email} and phone: {PhoneNumber}", user.Email, user.PhoneNumber);
                }
                else
                {
                    logger.LogError("Error creating admin user: {Errors}", string.Join(", ", result.Errors));
                }
            }
            else
            {
                logger.LogInformation("Admin user already exists.");
            }
        }
    }
}
