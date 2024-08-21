using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Helen.Domain.GenericResponse;
using Helen.Domain.MTN.SMS.Response;
using Helen.Domain.Paystack;
using Helen.Domain.Paystack.Request;
using Helen.Domain.Paystack.Response;
using Helen.Service.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    /*
    public interface IPaystackService
    {
        Task<GenericResponse<PaystackInitializeTransactionResponse>> InitializeTransactionAsync(PaystackInitializeTransactionRequest request);
    }

    public class PaystackService : IPaystackService
    {
        private readonly IHttpService _httpService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaystackService> _logger;
        private readonly IUtility _utility;

        public PaystackService(IHttpService httpService, IConfiguration configuration, ILogger<PaystackService> logger, IUtility utility)
        {
            _httpService = httpService;
            _configuration = configuration;
            _logger = logger;
            _utility = utility;
        }

        public async Task<GenericResponse<PaystackInitializeTransactionResponse>> InitializeTransactionAsync(PaystackInitializeTransactionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "SMS request cannot be null");
            }

            var response = new GenericResponse<PaystackInitializeTransactionResponse>();

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

                var apiResponse = await _utility.Response<SendSmsResponse>(url, HttpMethod.Post, request);

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
    }*/
}

