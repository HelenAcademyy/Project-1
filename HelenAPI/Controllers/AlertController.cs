using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Helen.Domain.GenericResponse;
using Helen.Service;
using Helen.Domain.MTN.SMS.Response;

namespace HelenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertController : ControllerBase
    {
        private readonly IAlertService _alertService;
        private readonly ILogger<AlertController> _logger;

        public AlertController(IAlertService alertService, ILogger<AlertController> logger)
        {
            _alertService = alertService;
            _logger = logger;
        }

        /*[[HttpPost("send-sms")]
        ProducesResponseType(typeof(GenericResponse<SendSmsResponse>), 200)]
        [ProducesResponseType(typeof(GenericResponse<SendSmsResponse>), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> SendSms()
        {
            try
            {
                var response = new GenericResponse<SendSmsResponse>();
                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                else
                {
                    _logger.LogError($"Failed to send SMS: {response.Message}");
                    return BadRequest(response); 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }*/
    }

}
