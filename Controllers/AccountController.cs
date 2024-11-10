using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using System.Net;
using YogeshFurnitureAPI.Interface.Account;
using YogeshFurnitureAPI.Model.Account;
using YogeshFurnitureAPI.Model.ResponseModel;

namespace YogeshFurnitureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(ILogger<AccountController> logger, IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(typeof(ErrorMessageWrapper), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _accountService.LoginAsync(request);
                if (result.IsSuccessfull)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(AccountController), nameof(Login), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Login failed." });
            }
        }

        [HttpPost("CreateYogeshFurnitureUsers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(typeof(ErrorMessageWrapper), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateYogeshFurnitureUsers([FromBody] CreateYogeshFurnitureUsers request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
                {
                    return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Email or Phone must be provided." });
                }
                var result = await _accountService.CreateYogeshFurnitureUsersAsync(request);
                if (result.IsSuccessfull)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(AccountController), nameof(CreateYogeshFurnitureUsers), ex.Message);
                return BadRequest(new ErrorMessageWrapper { ErrorMessage = "Registration failed." });
            }
        }

    }
}
