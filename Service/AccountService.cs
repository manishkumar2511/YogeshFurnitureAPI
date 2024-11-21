using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using YogeshFurnitureAPI.Helper.Services;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Model.Account;
using YogeshFurnitureAPI.Model.ResponseModel;

namespace YogeshFurnitureAPI.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<YogeshFurnitureUsers> _userManager;
        private readonly SignInManager<YogeshFurnitureUsers> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWTService _jwtService;

        public AccountService(UserManager<YogeshFurnitureUsers> userManager, SignInManager<YogeshFurnitureUsers> signInManager,
            RoleManager<IdentityRole> roleManager, JWTService jwtService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public async Task<ResponseMessage> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                return new ResponseMessage("Username and password are required.", null, false, StatusCodes.Status400BadRequest);  
            }

            var user = await _userManager.FindByEmailAsync(request.UserName)
                       ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.UserName);

            if (user == null)
            {
                return new ResponseMessage("Invalid credentials.", null, false, StatusCodes.Status400BadRequest);  
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!signInResult.Succeeded)
            {
                return new ResponseMessage("Invalid credentials.", null, false, StatusCodes.Status400BadRequest);  
            }

            var role = await _userManager.GetRolesAsync(user);
            if (role == null || !role.Any())
            {
                return new ResponseMessage("User does not have any assigned roles.", null, false, StatusCodes.Status400BadRequest);  // StatusCode 400
            }

            string token = _jwtService.GenerateJwtToken(user, role);

            var userDto = new CreateUserDTO
            {
                Token = token,
                Username = user.UserName,
                Role = role.FirstOrDefault()
            };

            return new ResponseMessage("Login successful.", userDto, true, StatusCodes.Status200OK);  
        }



        public async Task<ResponseMessage> CreateYogeshFurnitureUsersAsync(CreateYogeshFurnitureUsers request)
        {
            string userName = !string.IsNullOrEmpty(request.Email) ? request.Email : request.PhoneNumber;
            if (string.IsNullOrEmpty(userName))
            {
                return new ResponseMessage("Error: Email or PhoneNumber is required to register a user.", null, false, (int)HttpStatusCode.BadRequest);
            }

            // Check if the user already exists
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
                return new ResponseMessage("User with the provided email or phone number already exists.", null, false, (int)HttpStatusCode.Conflict);
            }

            // Ensure the "User" role exists, if not, create it
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                {
                    return new ResponseMessage("Failed to create User role. " + string.Join(", ", roleResult.Errors.Select(e => e.Description)), null, false, (int)HttpStatusCode.InternalServerError);
                }
            }

            // Generate a random password if not provided
            string password = Guid.NewGuid().ToString("N").Substring(0, 8); // Generates a random password of 8 characters

            var user = new YogeshFurnitureUsers
            {
                UserName = userName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.Now
            };

            // Hash the generated password
            var passwordHasher = new PasswordHasher<YogeshFurnitureUsers>();
            var hashedPassword = passwordHasher.HashPassword(user, password);

            // Create the user with the hashed password
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Assign the "User" role to the new user
                var roleAssignResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleAssignResult.Succeeded)
                {
                    return new ResponseMessage("User registration successful, but failed to assign role: " + string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)), user, true, (int)HttpStatusCode.OK);
                }

                // Add claims if email or phone number is provided
                if (!string.IsNullOrEmpty(user.Email))
                {
                    var emailClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
                    if (!emailClaimResult.Succeeded)
                    {
                        return new ResponseMessage("User registration successful, but failed to add email claim: " + string.Join(", ", emailClaimResult.Errors.Select(e => e.Description)), user, true, (int)HttpStatusCode.OK);
                    }
                }

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                {
                    var phoneClaimResult = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
                    if (!phoneClaimResult.Succeeded)
                    {
                        return new ResponseMessage("User registration successful, but failed to add phone number claim: " + string.Join(", ", phoneClaimResult.Errors.Select(e => e.Description)), user, true, (int)HttpStatusCode.OK);
                    }
                }

                // You can later integrate email/SMS to send the generated password
                // For now, this is a response message stating that a password was generated
                return new ResponseMessage("User registration successful with claims and role assigned. A random password has been generated.", new { user = user, UserName = user.UserName, Password = password }, true, (int)HttpStatusCode.Created);
            }

            return new ResponseMessage("Registration failed. " + string.Join(", ", result.Errors.Select(e => e.Description)), null, false, (int)HttpStatusCode.BadRequest);
        }


    }
}
