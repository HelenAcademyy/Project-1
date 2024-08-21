using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helen.Domain.BulkSms.Request;
using Helen.Domain.Google.Response;
using Helen.Domain.Invites;
using Helen.Service;
using Microsoft.Extensions.Logging;
using static EmailService;

namespace SendNotifications.Services
{
    public interface ISendNotificationService
    {
        Task NotifyCustomersAsync();
    }

    public class SendNotificationService : ISendNotificationService
    {
        private readonly IInviteService _inviteService;
        private readonly ILogger<SendNotificationService> _logger;
        private readonly IBulkSmsService _bulkSmsService;
        private readonly IEmailService _emailService;

        public SendNotificationService(IInviteService inviteService, ILogger<SendNotificationService> logger, IBulkSmsService bulkSmsService, IEmailService emailService)
        {
            _inviteService = inviteService;
            _logger = logger;
            _bulkSmsService = bulkSmsService;
            _emailService = emailService;
        }

        public async Task NotifyCustomersAsync()
        {
            try
            {
                var notificationList = await _inviteService.MatchCustomersWithLocationsAsync();

                if (notificationList?.Data == null || !notificationList.Data.Any())
                {
                    _logger.LogWarning("No matching customer-location data available.");
                    return;
                }

                foreach (var customerDetail in notificationList.Data)
                {
                    if (customerDetail.MatchingLocations == null) break;

                    foreach (var location in customerDetail.MatchingLocations.Where(location => customerDetail.Customer?.Location == location.Area))
                    {
                        if (!customerDetail.Customer.SendViaMail && !customerDetail.Customer.SendViaPhone) break;

                        if (customerDetail.Customer.SendViaMail)
                        {
                            await SendMailAsync(customerDetail.Customer.Email, location, customerDetail.Customer.Username);
                        }

                        if (customerDetail.Customer.SendViaPhone)
                        {
                            await SendMessageAsync(customerDetail.Customer.PhoneNumber, location);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending notifications.");
            }
        }

        private async Task SendMailAsync(string customerEmail, CustomerLocation location, string username)
        {
            try
            {
                var emailRequest = new EmailRequest
                {
                    To = new List<string> { customerEmail },
                    Body = CreateHtml(customerEmail, location, username),
                    Subject = "Check Out This Location!",
                    SentDate = DateTime.Now
                };

                await _emailService.SendEmailAsync(emailRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while sending email to {customerEmail}: {ex.Message}");
            }
        }

        private async Task SendMessageAsync(string customerPhoneNumber, CustomerLocation location)
        {
            try
            {
                var smsRequest = new SendSmsRequest
                {
                    To = customerPhoneNumber,
                    Body = CreateMessage(customerPhoneNumber, location)
                };

                await _bulkSmsService.SendSMSAsync(smsRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while sending SMS to {customerPhoneNumber}: {ex.Message}");
            }
        }

        private string CreateHtml(string customerName, CustomerLocation location, string username)
        {
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "inviteMail.html");

            if (!File.Exists(templatePath))
            {
                throw new FileNotFoundException($"Template file not found at path: {templatePath}");
            }

            string template = File.ReadAllText(templatePath, Encoding.UTF8);

            string result = template
                .Replace("{CustomerName}", username ?? "N/A")
                .Replace("{LocationName}", location.Name ?? "N/A")
                .Replace("{Address}", location.Location ?? "N/A")
                .Replace("{OpenTime}", FormatTime(location.WeekdayOpenTime) ?? "N/A")
                .Replace("{CloseTime}", FormatTime(location.WeekdayCloseTime) ?? "N/A")
                .Replace("{SaturdayOpenTime}", FormatTime(location.SaturdayOpenTime) ?? "N/A")
                .Replace("{SaturdayCloseTime}", FormatTime(location.SaturdayCloseTime) ?? "N/A")
                .Replace("{SundayOpenTime}", FormatTime(location.SundayOpenTime) ?? "N/A")
                .Replace("{SundayCloseTime}", FormatTime(location.SundayCloseTime) ?? "N/A")
                .Replace("{Type}", location.Type ?? "N/A")
                .Replace("{AdditionalInformation}", location.ExtraInformation ?? "None");


            return result;
        }



        private string CreateMessage(string customerName, CustomerLocation location)
        {
            return $"Hi {customerName},\n\n" +
                   $"Check out {location.Name}!\n\n" +
                   $"Details:\n" +
                   $"- Location: {location.Location}\n" +
                   $"- Open Time (Weekdays): {FormatTime(location.WeekdayOpenTime)}\n" +
                   $"- Close Time (Weekdays): {FormatTime(location.WeekdayCloseTime)}\n" +
                   $"- Saturday Open Time: {FormatTime(location.SaturdayOpenTime)}\n" +
                   $"- Saturday Close Time: {FormatTime(location.SaturdayCloseTime)}\n" +
                   $"- Sunday Open Time: {FormatTime(location.SundayOpenTime)}\n" +
                   $"- Sunday Close Time: {FormatTime(location.SundayCloseTime)}\n" +
                   $"- Type: {location.Type ?? "N/A"}\n" +
                   $"- Additional Info: {location.ExtraInformation ?? "None"}\n\n" +
                   $"Enjoy your time there!\n\n" +
                   $"Best regards,\nYour Event Team";
        }

        private string FormatTime(DateTime? time)
        {
            return time?.ToString("hh:mm tt") ?? "N/A";
        }
    }
}
