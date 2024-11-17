using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YogeshFurnitureAPI.Interface.Notification;
using YogeshFurnitureAPI.Model.NotificationModel;
using YogeshFurnitureAPI.Model.ResponseModel;

namespace YogeshFurnitureAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public NotificationController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorMessageWrapper))]
        [HttpPost("SendQuery")]
        public async Task<IActionResult> SendQuery([FromBody] SendEmailDTO emailDto)
        {
            if (string.IsNullOrEmpty(emailDto.To) || string.IsNullOrEmpty(emailDto.Body))
                return BadRequest("Email and Body are required.");

            var isSent = await _emailService.SendEmailAsync(emailDto);

            if (isSent)
                return Ok("Email sent successfully.");
            else
                return StatusCode(500, "Failed to send email. Please try again.");
        }

    }
}
