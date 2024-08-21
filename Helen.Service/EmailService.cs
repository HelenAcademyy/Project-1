using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Helen.Domain.Google.Response;
using Helen.Domain.Invites.Response;
using Helen.Repository.Models;
using Helen.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static EmailService;

public class EmailService : IEmailService
{
    public interface IEmailService
    {
        Task<List<GenericResponse<EmailResponse>>> SendEmailAsync(EmailRequest request);
    }

    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly HelenDbContext _dbContext;
    private readonly IInviteService _inviteService;
    private readonly string[] Scopes = { GmailService.Scope.GmailSend };
    private readonly string ApplicationName = "Helen";

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HelenDbContext dbContext, IInviteService inviteService)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _inviteService = inviteService ?? throw new ArgumentNullException(nameof(inviteService));
    }

    public async Task<List<GenericResponse<EmailResponse>>> SendEmailAsync(EmailRequest request)
    {
        var emailDataList = new List<EmailData>();
        var responses = new List<GenericResponse<EmailResponse>>();
        var sentDate = DateTime.UtcNow;

        try
        {
            //var (emailBody, recipientEmails) = await GenerateEmailBody(request);

            //if (recipientEmails.Count == 0)
            //{
            //    throw new Exception("No recipient emails found.");
            //}

            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            //foreach (var recipientEmail in recipientEmails)
            //{
                try
                {
                    var message = CreateEmail(request.To.FirstOrDefault(), _configuration["Gmail:Email"], request.Subject, request.Body);
                    var gmailRequest = service.Users.Messages.Send(message, "me");
                    var response = await gmailRequest.ExecuteAsync();

                    var emailData = new EmailData
                    {
                        To = request.To.FirstOrDefault(),
                        Subject = request.Subject,
                        Body = request.Body,
                        SentDate = sentDate,
                        ResponseMessage = response.Id,
                        IsSuccessful = true,
                        Message = "Email sent successfully"
                    };

                    emailDataList.Add(emailData);
                    responses.Add(new GenericResponse<EmailResponse>
                    {
                        IsSuccessful = true,
                        ResponseCode = 200,
                        Message = "Email sent successfully",
                        Data = new EmailResponse
                        {
                            To = request.To.FirstOrDefault(),
                            Subject = request.Subject,
                            SentDate = sentDate
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending email to {request.To.FirstOrDefault()}");

                    var emailData = new EmailData
                    {
                        To = request.To.FirstOrDefault(),
                        Subject = request.Subject,
                        Body = request.Body,
                        SentDate = sentDate,
                        IsSuccessful = false,
                        Message = $"Failed to send email to {request.To.FirstOrDefault()}: {ex.Message}"
                    };

                    emailDataList.Add(emailData);
                    responses.Add(new GenericResponse<EmailResponse>
                    {
                        IsSuccessful = false,
                        ResponseCode = 500,
                        Message = $"Error sending email to {request.To.FirstOrDefault()}: {ex.Message}",
                        Data = null
                    });
                }
            //}

            await _dbContext.EmailData.AddRangeAsync(emailDataList);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Emails sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during email sending process");

            var emailData = new EmailData
            {
                To = "",
                Subject = request.Subject,
                Body = "",
                SentDate = sentDate,
                IsSuccessful = false,
                Message = "Failed to send email"
            };

            emailDataList.Add(emailData);
            await _dbContext.EmailData.AddRangeAsync(emailDataList);
            await _dbContext.SaveChangesAsync();

            responses.Add(new GenericResponse<EmailResponse>
            {
                IsSuccessful = false,
                ResponseCode = 500,
                Message = $"Error sending email: {ex.Message}",
                Data = null
            });
        }

        return responses;
    }

    


    private Message CreateEmail(string to, string from, string subject, string bodyText)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("", from));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = bodyText };

        using (var memoryStream = new MemoryStream())
        {
            emailMessage.WriteTo(memoryStream);
            var rawMessage = Convert.ToBase64String(memoryStream.ToArray());
            return new Message { Raw = rawMessage };
        }
    }
}
