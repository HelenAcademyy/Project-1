using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.Invites.Response;
using Helen.Domain.GenericResponse;
using Helen.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Helen.Repository;

namespace HelenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("AddUser")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<UserData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddUser([FromBody] IEnumerable<UserData> users)
        {
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No data provided.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status400BadRequest,
                    Message = "No data provided.",
                    Data = null
                });
            }

            try
            {
                var response = await _userService.AddUsersAsync(users);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred while adding users.",
                    Data = null
                });
            }
        }

        [HttpPut("UpdateUser")]
        [ProducesResponseType(typeof(GenericResponse<UserData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] UserData user)
        {
            if (user == null)
            {
                _logger.LogWarning("No data provided.");
                return BadRequest(new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status400BadRequest,
                    Message = "No data provided.",
                    Data = null
                });
            }

            try
            {
                var response = await _userService.UpdateUserAsync(user);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred while updating the user.",
                    Data = null
                });
            }
        }

        [HttpGet("GetAllUsers")]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<UserData>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var response = await _userService.GetAllUsersAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve users.");
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<string>
                {
                    IsSuccessful = false,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Message = "An unexpected error occurred while retrieving users.",
                    Data = null
                });
            }
        }
    }
}
