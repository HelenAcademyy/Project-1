using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
using Helen.Domain.Invites.Response;
using Helen.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    public interface IUserService
    {
        Task<GenericResponse<IEnumerable<UserData>>> AddUsersAsync(IEnumerable<UserData> users);
        Task<GenericResponse<UserData>> UpdateUserAsync(UserData user);
        Task<GenericResponse<IEnumerable<UserData>>> GetAllUsersAsync();
        Task<GenericResponse<UserData>> GetUserByUsernameAsync(string username);
    }

    public class UserService : IUserService
    {
        private readonly HelenDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(HelenDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<GenericResponse<IEnumerable<UserData>>> AddUsersAsync(IEnumerable<UserData> users)
        {
            var request = JsonSerializer.Serialize(users);

            try
            {
                var existingUsernames = await _dbContext.UserData
                    .AsNoTracking()
                    .Where(u => users.Select(user => user.Username).Contains(u.Username))
                    .Select(u => u.Username)
                    .ToListAsync();

                var usersToAdd = users.Where(user => !existingUsernames.Contains(user.Username)).ToList();
                var existingUsers = users.Where(user => existingUsernames.Contains(user.Username)).ToList();

                var response = new GenericResponse<IEnumerable<UserData>>
                {
                    IsSuccessful = true
                };

                if (existingUsers.Any())
                {
                    response.ResponseCode = usersToAdd.Any() ? 207 : 409;
                    response.Message = $"Some users already exist: {string.Join(", ", existingUsers.Select(u => u.Username))}";
                    response.Data = existingUsers; 
                }

                if (usersToAdd.Any())
                {
                    await _dbContext.UserData.AddRangeAsync(usersToAdd);
                    await _dbContext.SaveChangesAsync();

                    response.ResponseCode = existingUsers.Any() ? 207 : 201; 
                    response.Message = existingUsers.Any() ? "Some users were already created, and new users were added successfully." : "Users added successfully";
                    response.Data = usersToAdd; 
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not add users: {Details}", request);
                return new GenericResponse<IEnumerable<UserData>>
                {
                    ResponseCode = 500, 
                    IsSuccessful = false,
                    Message = "An error occurred while adding users",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<UserData>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _dbContext.UserData
                    .AsNoTracking()
                    .ToListAsync();

                return new GenericResponse<IEnumerable<UserData>>
                {
                    ResponseCode = 200,
                    IsSuccessful = true,
                    Message = "Users fetched successfully",
                    Data = users
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch users");
                return new GenericResponse<IEnumerable<UserData>>
                {
                    ResponseCode = 500,
                    IsSuccessful = false,
                    Message = "An error occurred while fetching users",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<UserData>> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new GenericResponse<UserData>
                {
                    ResponseCode = 400,
                    IsSuccessful = false,
                    Message = "Username cannot be null or empty",
                    Data = null
                };
            }

            try
            {
                var user = await _dbContext.UserData
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return new GenericResponse<UserData>
                    {
                        ResponseCode = 404,
                        IsSuccessful = false,
                        Message = "User not found",
                        Data = null
                    };
                }

                return new GenericResponse<UserData>
                {
                    ResponseCode = 200,
                    IsSuccessful = true,
                    Message = "User fetched successfully",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not fetch user by username: {Username}", username);
                return new GenericResponse<UserData>
                {
                    ResponseCode = 500,
                    IsSuccessful = false,
                    Message = "An error occurred while fetching the user",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<UserData>> UpdateUserAsync(UserData user)
        {
            if (user == null)
            {
                return new GenericResponse<UserData>
                {
                    ResponseCode = 400,
                    IsSuccessful = false,
                    Message = "User data cannot be null",
                    Data = null
                };
            }

            try
            {
                // Find the existing user by Username and Id
                var existingUser = await _dbContext.UserData
                    .FirstOrDefaultAsync(u => u.Username == user.Username);

                if (existingUser == null)
                {
                    return new GenericResponse<UserData>
                    {
                        ResponseCode = 404,
                        IsSuccessful = false,
                        Message = "User not found",
                        Data = null
                    };
                }

                // Update properties if they differ
                bool isUpdated = false;

                if (existingUser.PhoneNumber != user.PhoneNumber)
                {
                    existingUser.PhoneNumber = user.PhoneNumber;
                    isUpdated = true;
                }
                if (existingUser.Email != user.Email)
                {
                    existingUser.Email = user.Email;
                    isUpdated = true;
                }
                if (existingUser.Budget != user.Budget)
                {
                    existingUser.Budget = user.Budget;
                    isUpdated = true;
                }
                if (existingUser.IsSmoker != user.IsSmoker)
                {
                    existingUser.IsSmoker = user.IsSmoker;
                    isUpdated = true;
                }
                if (existingUser.ReminderFrequency != user.ReminderFrequency)
                {
                    existingUser.ReminderFrequency = user.ReminderFrequency;
                    isUpdated = true;
                }
                if (existingUser.Status != user.Status)
                {
                    existingUser.Status = user.Status;
                    isUpdated = true;
                }
                if (existingUser.ReminderTime != user.ReminderTime)
                {
                    existingUser.ReminderTime = user.ReminderTime;
                    isUpdated = true;
                }
                if (existingUser.SendViaMail != user.SendViaMail)
                {
                    existingUser.SendViaMail = user.SendViaMail;
                    isUpdated = true;
                }
                if (existingUser.SendViaPhone != user.SendViaPhone)
                {
                    existingUser.SendViaPhone = user.SendViaPhone;
                    isUpdated = true;
                }
                if (existingUser.Location != user.Location.ToLower())
                {
                    existingUser.Location = user.Location.ToLower();
                    isUpdated = true;
                }

                // Save changes if updates were made
                if (isUpdated)
                {
                    await _dbContext.SaveChangesAsync();
                }

                return new GenericResponse<UserData>
                {
                    ResponseCode = 200,
                    IsSuccessful = true,
                    Message = isUpdated ? "User updated successfully" : "No changes detected",
                    Data = existingUser
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not update user: {Username}", user.Username);
                return new GenericResponse<UserData>
                {
                    ResponseCode = 500,
                    IsSuccessful = false,
                    Message = "An error occurred while updating the user",
                    Data = null
                };
            }
        }

    }
}
