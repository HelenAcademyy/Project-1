using System;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
using Helen.Domain.MTN.SMS.Request;
using Helen.Domain.MTN.SMS.Response;
using Helen.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MTNController : ControllerBase
    {
        /*private readonly ILogger<MTNController> _logger;
        private readonly IMTNService _MTNService;

        public MTNController(ILogger<MTNController> logger, IMTNService MTNService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _MTNService = MTNService ?? throw new ArgumentNullException(nameof(MTNService));
        }

        [HttpPost("send-sms")]
        [ProducesResponseType(typeof(SendSmsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("Received null SMS request.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status400BadRequest,
                    Message = "Request cannot be null.",
                    Data = null
                });
            }

            try
            {
                var response = await _MTNService.SendSms<SendSmsResponse>(request);

                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("SMS sending failed: {Message}", response.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                    {
                        IsSuccessful = false,
                        ResponseCode = StatusCodes.Status500InternalServerError,
                        Message = response.Message,
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred while sending SMS.",
                    Data = null
                });
            }
        }
    }*/
    }
}
