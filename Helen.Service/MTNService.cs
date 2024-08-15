using System;
using System.Net.Http;
using System.Threading.Tasks;
using Helen.Domain.GenericResponse;
using Helen.Domain.MTN.SMS.Request;
using Helen.Domain.MTN.SMS.Response;
using Helen.Service.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    public interface IMTNService
    {
        Task<GenericResponse<T>> SendSms<T>(SmsRequest request) where T : class;
    }
    public class MTNService : IMTNService
    {
        private readonly IUtility _utility;
        private readonly ILogger<MTNService> _logger;
        private readonly IConfiguration _configuration;

        public MTNService(IUtility utility, ILogger<MTNService> logger, IConfiguration configuration)
        {
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<GenericResponse<T>> SendSms<T>(SmsRequest request) where T : class
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "SMS request cannot be null");
            }

            var response = new GenericResponse<T>();

            try
            {
                string baseUrl = _configuration["SmsApi:BaseUrl"]
                    ?? throw new InvalidOperationException("Base URL configuration is missing");
                string endpoint = _configuration["SmsApi:Endpoint"]
                    ?? throw new InvalidOperationException("Endpoint configuration is missing");

                var uriBuilder = new UriBuilder(baseUrl.TrimEnd('/'))
                {
                    Path = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}"
                };

                string url = uriBuilder.ToString();

                // Send the SMS request using the utility
                var apiResponse = await _utility.Response<SendSmsResponse>(url, HttpMethod.Post, request);//Response<SendSmsResponse>(url, HttpMethod.Post, request);

                response.ResponseCode = apiResponse.ResponseCode;
                response.IsSuccessful = apiResponse.IsSuccessful;

                response.Message = response.IsSuccessful
                    ? "Request successful"
                    : apiResponse.ResponseCode.ToString().StartsWith("5")
                        ? "Server error occurred"
                        : "Request failed";

                if (!response.IsSuccessful && response.ResponseCode != 201)
                {
                    _logger.LogWarning("Received non-successful response code: {ResponseCode}", response.ResponseCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS due to: {Message}", ex.Message);

                response.ResponseCode = 500;
                response.IsSuccessful = false;
                response.Message = "An unexpected error occurred while sending SMS.";
            }

            return response;
        }
    }
}
