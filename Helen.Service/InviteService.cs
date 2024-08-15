using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
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
    }

    public class InviteService : IInviteService
    {
        private readonly ILogger<InviteService> _logger;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly HelenDbContext _dbContext;
        private readonly int _batchSize;

        public InviteService(
            ILogger<InviteService> logger,
            IMemoryCache cache,
            IConfiguration configuration,
            HelenDbContext dbContext)
        {
            _logger = logger;
            _cache = cache;
            _configuration = configuration;
            _dbContext = dbContext;
            _batchSize = Convert.ToInt32(configuration["BatchProcessing:BatchSize"]);
        }

        public async Task<GenericResponse<IEnumerable<LocationNotificationData>>> GetAllLocationsAsync()
        {
            var cacheKey = "AllLocations";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<LocationNotificationData> locations))
            {
                try
                {
                    locations = await _dbContext.LocationNotificationData.AsNoTracking().ToListAsync();

                    bool isLive = Convert.ToBoolean(_configuration["Production:IsLive"]);

                    if (isLive)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Cache:AbsoluteExpirationMinutes"])),
                            SlidingExpiration = TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Cache:SlidingExpirationMinutes"]))
                        };
                        _cache.Set(cacheKey, locations, cacheEntryOptions);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to fetch locations.");
                    return new GenericResponse<IEnumerable<LocationNotificationData>>
                    {
                        IsSuccessful = false,
                        ResponseCode = 500,
                        Message = "An unexpected error occurred.",
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
                var existingNames = await _dbContext.LocationNotificationData
                    .AsNoTracking()
                    .Where(l => locations.Select(loc => loc.Name).Contains(l.Name))
                    .Select(l => l.Name)
                    .ToListAsync();

                var existingNamesSet = new HashSet<string>(existingNames);

                var locationsToAdd = locations.Where(loc => !existingNamesSet.Contains(loc.Name)).ToList();

                if (locationsToAdd.Any())
                {
                    await _dbContext.LocationNotificationData.AddRangeAsync(locationsToAdd);
                    await _dbContext.SaveChangesAsync();
                }

                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = true,
                    ResponseCode = 201,
                    Message = "Locations added successfully",
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
                    Message = "An unexpected error occurred.",
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
                var locationNames = locations.Select(loc => loc.Name).ToHashSet();
                var existingLocations = await _dbContext.LocationNotificationData
                    .Where(l => locationNames.Contains(l.Name))
                    .ToListAsync();

                var existingLocationsDict = existingLocations.ToDictionary(l => l.Name);
                var locationsToUpdate = new List<LocationNotificationData>();
                var locationsNotFound = new List<LocationNotificationData>();

                foreach (var loc in locations)
                {
                    if (existingLocationsDict.TryGetValue(loc.Name, out var existingLoc))
                    {
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
                    Message = "Locations updated successfully",
                    Data = locationsToUpdate.Concat(locationsNotFound) 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update locations.");
                return new GenericResponse<IEnumerable<LocationNotificationData>>
                {
                    IsSuccessful = false,
                    ResponseCode = 500,
                    Message = "An unexpected error occurred.",
                    Data = null
                };
            }
        }

    }
}
