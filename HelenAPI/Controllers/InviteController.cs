using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
using Helen.Domain.Invites.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Helen.Service;

namespace HelenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InviteController : ControllerBase
    {
        private readonly ILogger<InviteController> _logger;
        private readonly IInviteService _inviteService;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly int _batchSize;

        public InviteController(
            ILogger<InviteController> logger,
            IInviteService inviteService,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _logger = logger;
            _inviteService = inviteService;
            _cache = cache;
            _configuration = configuration;
            _batchSize = _configuration.GetValue<int>("BatchProcessing:BatchSize");
        }

        [HttpGet("get-all-locations")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllLocations()
        {
            var cacheKey = "AllLocations";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<LocationNotificationData> locations))
            {
                try
                {
                    var response = await _inviteService.GetAllLocationsAsync();
                    locations = response.Data;

                    bool isLive = Convert.ToBoolean(_configuration["Production:IsLive"]);
                    if (isLive && locations != null)
                    {
                        var absoluteExpirationMinutes = _configuration.GetValue<int>("Cache:AbsoluteExpirationMinutes");
                        var slidingExpirationMinutes = _configuration.GetValue<int>("Cache:SlidingExpirationMinutes");

                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(absoluteExpirationMinutes),
                            SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes)
                        };
                        _cache.Set(cacheKey, locations, cacheEntryOptions);
                    }

                    return Ok(new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = true,
                        ResponseCode = 200,
                        Message = $"{locations?.Count() ?? 0} record(s) retrieved.",
                        Data = locations
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch locations.");
                    return StatusCode(500, new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = false,
                        ResponseCode = 500,
                        Message = "An unexpected error occurred.",
                        Data = null
                    });
                }
            }

            return Ok(new GenericResponse<IEnumerable<LocationNotificationData>>
            {
                IsSuccessful = true,
                ResponseCode = 200,
                Message = $"{locations?.Count() ?? 0} record(s) retrieved from cache.",
                Data = locations
            });
        }

        [HttpPost("add-location")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddLocation([FromBody] IEnumerable<LocationNotificationData> locations)
        {
            if (locations == null || !locations.Any())
            {
                _logger.LogWarning("No data provided.");
                return BadRequest(new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "No data provided.",
                    Data = null
                });
            }

            try
            {
                var response = await _inviteService.AddLocationAsync(locations);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to add locations. Response: {Response}", response.Message);
                    return StatusCode(response.ResponseCode, new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = false,
                        ResponseCode = response.ResponseCode,
                        Message = response.Message,
                        Data = null
                    });
                }

                _logger.LogInformation("Locations added successfully.");
                return CreatedAtAction(nameof(GetAllLocations), new { }, response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add locations.");
                return StatusCode(500, new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred.",
                    Data = null
                });
            }
        }

        [HttpPut("update-location")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<LocationNotificationData>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocation([FromBody] IEnumerable<LocationNotificationData> locations)
        {
            if (locations == null || !locations.Any())
            {
                _logger.LogWarning("No data provided.");
                return BadRequest(new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "No data provided.",
                    Data = null
                });
            }

            try
            {
                var response = await _inviteService.UpdateLocationAsync(locations);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to update locations. Response: {Response}", response.Message);
                    return StatusCode(response.ResponseCode, new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = false,
                        ResponseCode = response.ResponseCode,
                        Message = response.Message,
                        Data = null
                    });
                }

                _logger.LogInformation("Locations updated successfully.");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update locations.");
                return StatusCode(500, new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred.",
                    Data = null
                });
            }
        }

        
        /*[HttpPost("UpdateLocationFromFile")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateLocationFromFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("No file uploaded.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "No file uploaded.",
                    Data = null
                });
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                _logger.LogWarning("Invalid file type.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "Invalid file type. Only Excel files are allowed.",
                    Data = null
                });
            }

            try
            {
                var response = await _inviteService.UpdateLocationFromFileAsync(file);

                if (!response.IsSuccessful)
                {
                    _logger.LogWarning("Failed to process file. Response: {Response}", response.Message);
                    return StatusCode(response.ResponseCode.Value, new GenericResponse<string>
                    {
                        IsSuccessful = false,
                        ResponseCode = response.ResponseCode.Value,
                        Message = response.Message,
                        Data = null
                    });
                }

                _logger.LogInformation("File processed and data saved successfully.");
                return Ok(new GenericResponse<string>
                {
                    IsSuccessful = true,
                    ResponseCode = 200,
                    Message = "File processed and data saved successfully.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process Excel file.");
                return StatusCode(500, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred.",
                    Data = null
                });
            }
        }*/
    }
}
