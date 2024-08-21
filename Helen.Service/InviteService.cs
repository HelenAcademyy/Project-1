using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.Enum;
using Helen.Domain.GenericResponse;
using Helen.Domain.Invites;
using Helen.Domain.Invites.Request;
using Helen.Domain.Invites.Response;
using Helen.Repository;
using Helen.Service.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    public interface IInviteService
    {
        Task<GenericResponse<IEnumerable<LocationNotificationData>>> UpdateLocationAsync(IEnumerable<LocationNotificationData> locations);
        Task<GenericResponse<IEnumerable<LocationNotificationData>>> GetAllLocationsAsync();
        Task<GenericResponse<IEnumerable<LocationNotificationData>>> AddLocationAsync(IEnumerable<LocationNotificationData> locations);
        Task<GenericResponse<IEnumerable<CustomerDetailsRequest>>> MatchCustomersWithLocationsAsync();
    }

    public class InviteService : IInviteService
    {
        private readonly ILogger<InviteService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly HelenDbContext _dbContext;
        private readonly IUtility _utility;

        public InviteService(
            ILogger<InviteService> logger,
            IMemoryCache cache,
            IConfiguration configuration,
            HelenDbContext dbContext,
            IUtility utility)
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
            _dbContext = dbContext;
            _utility = utility;
        }

        public async Task<GenericResponse<IEnumerable<LocationNotificationData>>> GetAllLocationsAsync()
        {
            const string cacheKey = "AllLocations";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<LocationNotificationData> locations))
            {
                try
                {
                    locations = await _dbContext.LocationNotificationData.AsNoTracking().ToListAsync();

                    if (bool.TryParse(_configuration["Production:IsLive"], out bool isLive) && isLive)
                    {
                        _cache.Set(cacheKey, locations, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(int.Parse(_configuration["Cache:AbsoluteExpirationMinutes"])),
                            SlidingExpiration = TimeSpan.FromMinutes(int.Parse(_configuration["Cache:SlidingExpirationMinutes"]))
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch locations.");
                    return new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = false,
                        ResponseCode = 500,
                        Message = "An unexpected error occurred while fetching locations.",
                        Data = null
                    };
                }
            }

            return new GenericResponse<IEnumerable<LocationNotificationData>>
            {
                IsSuccessful = true,
                ResponseCode = 200,
                Message = $"{locations.Count()} record(s) retrieved.",
                Data = locations
            };
        }

        public async Task<GenericResponse<IEnumerable<LocationNotificationData>>> AddLocationAsync(IEnumerable<LocationNotificationData> locations)
        {
            if (locations == null || !locations.Any())
            {
                _logger.LogWarning("No data provided.");
                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "No data provided.",
                    Data = null
                };
            }

            try
            {
                var existingNamesSet = new HashSet<string>(await _dbContext.LocationNotificationData
                    .AsNoTracking()
                    .Select(l => l.Name)
                    .ToListAsync());

                var locationsToAdd = locations
                    .Where(loc => !existingNamesSet.Contains(loc.Name))
                    .ToList();

                if (locationsToAdd.Any())
                {
                    await _dbContext.LocationNotificationData.AddRangeAsync(locationsToAdd);
                    await _dbContext.SaveChangesAsync();
                }

                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = true,
                    ResponseCode = 201,
                    Message = "Locations added successfully.",
                    Data = locationsToAdd
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add locations.");
                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred while adding locations.",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<LocationNotificationData>>> UpdateLocationAsync(IEnumerable<LocationNotificationData> locations)
        {
            if (locations == null || !locations.Any())
            {
                _logger.LogWarning("No data provided.");
                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 400,
                    Message = "No data provided.",
                    Data = null
                };
            }

            try
            {
                var locationNames = new HashSet<string>(locations.Select(loc => loc.Name));
                var existingLocations = await _dbContext.LocationNotificationData
                    .Where(l => locationNames.Contains(l.Name))
                    .ToDictionaryAsync(l => l.Name);

                var locationsToUpdate = new List<LocationNotificationData>();
                var locationsNotFound = new List<LocationNotificationData>();

                foreach (var loc in locations)
                {
                    if (existingLocations.TryGetValue(loc.Name, out var existingLoc))
                    {
                        // Update properties of existingLoc with loc values
                        existingLoc.WeekdayOpenTime = loc.WeekdayOpenTime;
                        existingLoc.WeekdayCloseTime = loc.WeekdayCloseTime;
                        existingLoc.SaturdayOpenTime = loc.SaturdayOpenTime;
                        existingLoc.SaturdayCloseTime = loc.SaturdayCloseTime;
                        existingLoc.SundayOpenTime = loc.SundayOpenTime;
                        existingLoc.SundayCloseTime = loc.SundayCloseTime;
                        existingLoc.Type = loc.Type;
                        existingLoc.Location = loc.Location;
                        existingLoc.ExtraInformation = loc.ExtraInformation;
                        existingLoc.Budget = loc.Budget;
                        existingLoc.AvailableForRent = loc.AvailableForRent;
                        existingLoc.RentPrice = loc.RentPrice;
                        existingLoc.DateAdded = loc.DateAdded;

                        locationsToUpdate.Add(existingLoc);
                    }
                    else
                    {
                        locationsNotFound.Add(loc);
                    }
                }

                if (locationsToUpdate.Any())
                {
                    _dbContext.LocationNotificationData.UpdateRange(locationsToUpdate);
                    await _dbContext.SaveChangesAsync();
                }

                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = true,
                    ResponseCode = 200,
                    Message = $"{locationsToUpdate.Count} location(s) updated successfully. {locationsNotFound.Count} location(s) not found.",
                    Data = locationsToUpdate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update locations.");
                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred while updating locations.",
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<IEnumerable<CustomerDetailsRequest>>> MatchCustomersWithLocationsAsync()
        {
            //bool sortByBudget = bool.TryParse(_configuration["Email:SortByBudget"], out var budget) && budget;
            //bool sortByLocation = bool.TryParse(_configuration["Email:SortByLocation"], out var location) && location;

            var customers = await _dbContext.UserData
                .Where(u => u.Status == ProfileStatus.Active)
                .Select(u => new CustomerDetail
                {
                    Username = u.Username,
                    Age = DateTime.Now.Year - u.DateOfBirth.Year - (DateTime.Now.DayOfYear < u.DateOfBirth.DayOfYear ? 1 : 0),
                    Status = (ProfileStatus)u.Status,
                    ReminderTime = u.ReminderTime,
                    SendViaMail = u.SendViaMail,
                    Email = u.Email,
                    Budget = u.Budget,
                    IsSmoker = u.IsSmoker,
                    Location = u.Location,
                    PhoneNumber = u.PhoneNumber,
                    ReminderFrequency = (ReminderFrequency)u.ReminderFrequency
                })
                .ToListAsync();

            var locations = await _dbContext.LocationNotificationData
                .Where(l => !string.IsNullOrEmpty(l.Location))
                .ToListAsync();

            var result = customers.Select(customer =>
            {
                var matchingLocations = locations
                    .Where(l => l.Area.Equals(customer.Location, StringComparison.OrdinalIgnoreCase))
                    .Select(l => new CustomerLocation
                    {
                        Name = l.Name,
                        WeekdayOpenTime = l.WeekdayOpenTime,
                        WeekdayCloseTime = l.WeekdayCloseTime,
                        SaturdayOpenTime = l.SaturdayOpenTime,
                        SaturdayCloseTime = l.SaturdayCloseTime,
                        SundayOpenTime = l.SundayOpenTime,
                        SundayCloseTime = l.SundayCloseTime,
                        Type = l.Type,
                        Location = l.Location,
                        Area = l.Area,
                        ExtraInformation = l.ExtraInformation,
                        Budget = l.Budget,
                        DateAdded = l.DateAdded
                    })
                    .ToList();

                return new CustomerDetailsRequest
                {
                    Customer = customer,
                    MatchingLocations = matchingLocations
                };
            }).ToList();

            //if (sortByBudget)
            //    result = result.OrderBy(r => r.Customer.Budget).ToList();

            //if (sortByLocation)
            //    result = result.OrderBy(r => r.Customer.Location).ToList();

            return new GenericResponse<IEnumerable<CustomerDetailsRequest>>
            {
                IsSuccessful = true,
                ResponseCode = 200,
                Message = "Customer and location data matched successfully.",
                Data = result
            };
        }
    }
}
