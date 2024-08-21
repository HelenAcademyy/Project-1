using System;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
using Helen.Domain.Paystack.Request;
using Helen.Domain.Paystack.Response;
using Helen.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaystackController : ControllerBase
    {
        /*private readonly IPaystackService _paystackService;
        private readonly ILogger<PaystackController> _logger;

        public PaystackController(IPaystackService paystackService, ILogger<PaystackController> logger)
        {
            _paystackService = paystackService ?? throw new ArgumentNullException(nameof(paystackService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("initialize-transaction")]
        [ProducesResponseType(typeof(GenericResponse<PaystackInitializeTransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InitializeTransaction([FromBody] PaystackInitializeTransactionRequest request)
        {
            if (request == null)
            {
                _logger.LogWarning("InitializeTransaction request is null.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status400BadRequest,
                    Message = "Request body cannot be null."
                });
            }

            try
            {
                var response = await _paystackService.InitializeTransactionAsync(request);

                if (response.IsSuccessful)
                {
                    return Ok(response);
                }
                else
                {
                    _logger.LogWarning("Transaction initialization failed with response code {ResponseCode}: {Message}", response.ResponseCode, response.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                    {
                        IsSuccessful = false,
                        ResponseCode = response.ResponseCode,
                        Message = response.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize transaction.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred while initializing the transaction."
                });
            }
        }
    }*/
    }
}
