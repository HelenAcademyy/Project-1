using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Helen.Domain.BulkSms.Request;
using Helen.Domain.BulkSms.Response;
using Helen.Domain.Invites.Response;
using Helen.Domain.MTN.SMS.Request;
using Helen.Domain.MTN.SMS.Response;
using Helen.Repository.Models;
using Helen.Service.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Helen.Service
{
    public interface IBulkSmsService
    {
        Task<GenericResponse<SmsResponse>> SendSMSAsync(SendSmsRequest request);
    }

    public class BulkSmsService : IBulkSmsService
    {
        private readonly IConfiguration _configuration;
        private readonly IUtility _utility;
        private readonly ILogger<BulkSmsService> _logger;
        private readonly HelenDbContext _dbContext;

        public BulkSmsService(IConfiguration configuration, IUtility utility, ILogger<BulkSmsService> logger, HelenDbContext dbContext)
        {
            _configuration = configuration;
            _utility = utility;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<GenericResponse<SmsResponse>> SendSMSAsync(SendSmsRequest request)
        {
            var response = new GenericResponse<SmsResponse>();

            try
            {
                string baseUrl = _configuration["BulkSms:BaseUrl"];
                string endpoint = _configuration["BulkSms:SendSms"];
                string from = _configuration["BulkSms:From"];
                string gateway = _configuration["BulkSms:GateWay"];
                string appendSender = _configuration["BulkSms:AppendSender"];
                string callbackUrl = _configuration["BulkSms:CallbackUrl"];
                string customerReference = _configuration["BulkSms:CustomerReference"];
                string apiToken = _configuration["BulkSms:ApiToken"];
                string url = baseUrl + endpoint;

                var payload = new SmsRequest
                {
                    from = from,
                    api_token = apiToken,
                    customerReference = customerReference,
                    body = request.Body,
                    callbackUrl = callbackUrl,
                    gateway = gateway,
                    to = request.To
                };

                _logger.LogInformation("Sending SMS with payload: {Payload}", JsonSerializer.Serialize(payload));

                var httpResponse = _utility.Response<SmsResponse>(url, HttpMethod.Post, payload);
                response.Data = httpResponse.Result.Data;
                response.IsSuccessful = httpResponse.Result.IsSuccessful;
                response.Message = httpResponse.Result.IsSuccessful ? "Ok" : "Failed";
                response.ResponseCode = httpResponse.Result.ResponseCode;

                var responseData = response?.Data?.data;

                var smsData = new SmsData
                {
                    Body = request.Body,
                    IsSuccessful = response.IsSuccessful,
                    Message = response.Message,
                    ResponseCode = response.ResponseCode,
                    ApiToken = apiToken,
                    CustomerReference = customerReference,
                    CallbackUrl = callbackUrl,
                    From = from,
                    Gateway = gateway,
                    To = request.To,
                    SentDate = DateTime.UtcNow,
                    Cost = responseData.cost,
                    Currency = responseData.currency,
                    Gateway_used = responseData.gateway_used,
                    Message_id = responseData.message_id,
                    Status = responseData.status
                };

                await _dbContext.SmsData.AddAsync(smsData);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending SMS");

                response.IsSuccessful = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.ResponseCode = 500; // Internal Server Error
            }

            return response;
        }
    }
}
