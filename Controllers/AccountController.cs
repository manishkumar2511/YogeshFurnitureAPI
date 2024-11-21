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
                {
                    return Ok(new Response(result.Data, null, true, StatusCodes.Status200OK));  
                }
                return BadRequest(new ResponseMessage(result.Message, result.Data, false, StatusCodes.Status400BadRequest)); 
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(AccountController), nameof(Login), ex.Message);
                return BadRequest(new ResponseMessage("Login failed.", null, false, StatusCodes.Status400BadRequest)); 
            }
        }

        [HttpPost("CreateUsers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
        [ProducesResponseType(typeof(ErrorMessageWrapper), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateUsers([FromBody] CreateYogeshFurnitureUsers request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.PhoneNumber))
                {
                    return BadRequest(new ResponseMessage("Email or Phone must be provided.", null, false, StatusCodes.Status400BadRequest)); // BadRequest with status code
                }

                var result = await _accountService.CreateYogeshFurnitureUsersAsync(request);
                if (result.IsSuccessfull)
                {
                    return Ok(new Response(result.Data, null, true, StatusCodes.Status200OK)); 
                }

                return BadRequest(new ResponseMessage(result.Message, result.Data, false, StatusCodes.Status400BadRequest)); // BadRequest with status code
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in {controller}/{action}: {message}", nameof(AccountController), nameof(CreateYogeshFurnitureUsers), ex.Message);
                return BadRequest(new ResponseMessage("Registration failed.", null, false, StatusCodes.Status400BadRequest)); // BadRequest with status code
            }
        }
    }
}
