
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YogeshFurnitureAPI.Helper.Services;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Model.Account;
using YogeshFurnitureAPI.Model.ResponseModel;

namespace YogeshFurnitureAPI.Service
{
    public class AccountService: IAccountService
    {
        private readonly UserManager<YogeshFurnitureUsers> _userManager;
        private readonly SignInManager<YogeshFurnitureUsers> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTService _jwtService;
        // Constructor with both UserManager and SignInManager injected
        public AccountService(UserManager<YogeshFurnitureUsers> userManager, SignInManager<YogeshFurnitureUsers> signInManager,
            RoleManager<IdentityRole> roleManager, JWTService jwtService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        public async Task<ResponseMessage> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserName);

            if (user == null)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.UserName);
            }

            if (user == null)
            {
                return new ResponseMessage("Invalid credentials.", null, false);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (signInResult.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string token = _jwtService.GenerateJwtToken(user, roles);
                var role = roles.FirstOrDefault();

                var userDto = new CreateUserDTO
                {
                    Token = token,
                    Username = user.UserName, 
                    Role = role 
                };

                return new ResponseMessage("Login successful.", userDto, true);
            }
            else
            {
                return new ResponseMessage("Invalid credentials.", null, false);
            }
        }

        public async Task<ResponseMessage> CreateYogeshFurnitureUsersAsync(CreateYogeshFurnitureUsers request)
        {
            string userName = !string.IsNullOrEmpty(request.Email) ? request.Email : request.PhoneNumber;
            if (string.IsNullOrEmpty(userName))
            {
                return new ResponseMessage("Error: Email or PhoneNumber is required to register a user.", null, false);
            }

            // Check if email or phone number already exists
            YogeshFurnitureUsers existingUser = null;
            if (!string.IsNullOrEmpty(request.Email))
            {
                existingUser = await _userManager.FindByEmailAsync(request.Email);
            }
            else if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            }

            if (existingUser != null)
            {
                return new ResponseMessage("User with the provided email or phone number already exists.", null, false);
            }

            // Ensure the "User" role exists, create it if not
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                {
                    return new ResponseMessage("Failed to create User role. " + string.Join(", ", roleResult.Errors.Select(e => e.Description)), null, false);
                }
            }

            var user = new YogeshFurnitureUsers
            {
                UserName = userName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Assign the "User" role
                var roleAssignResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleAssignResult.Succeeded)
                {
                    return new ResponseMessage("User registration successful, but failed to assign role: " + string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)), user, true);
                }

                // Add claims
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var emailClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                    if (!emailClaimResult.Succeeded)
                    {
                        return new ResponseMessage("User registration successful, but failed to add email claim: " + string.Join(", ", emailClaimResult.Errors.Select(e => e.Description)), user, true);
                    }
                }

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    var phoneClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                    if (!phoneClaimResult.Succeeded)
                    {
                        return new ResponseMessage("User registration successful, but failed to add phone number claim: " + string.Join(", ", phoneClaimResult.Errors.Select(e => e.Description)), user, true);
                    }
                }

                return new ResponseMessage("User registration successful with claims and role assigned.", user, true);
            }

            return new ResponseMessage("Registration failed. " + string.Join(", ", result.Errors.Select(e => e.Description)), null, false);
        }






    }
}
