using Helen.Domain.GenericResponse;
using Helen.Domain.Invites.Request;
using Helen.Domain.Invites.Response;
using Helen.Service;
using Helen.Service.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendNotifications.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using static EmailService;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a host builder
            var host = CreateHostBuilder(args).Build();

            // Run the host to start the background service
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Register DbContext
                    services.AddDbContext<HelenDbContext>(options =>
                        options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    // Register HttpClient
                    services.AddHttpClient(); // This registers HttpClient for DI

                    // Register other services
                    services.AddTransient<IHttpService, HttpService>();
                    services.AddTransient<IUserService, UserService>();
                    services.AddTransient<IInviteService, InviteService>();
                    services.AddTransient<IUtility, Utility>();
                    services.AddScoped<IEmailService, EmailService>();
                    services.AddTransient<IBulkSmsService, BulkSmsService>();
                    // Register ISendNotificationService
                    services.AddTransient<ISendNotificationService, SendNotificationService>();

                    // Register the hosted service
                    services.AddHostedService<MyBackgroundService>();

                    // Add memory cache and logging
                    services.AddMemoryCache();
                    services.AddLogging();
                });
    }

    public class MyBackgroundService : BackgroundService
    {
        private readonly ILogger<MyBackgroundService> _logger;
        private readonly ISendNotificationService _sendNotificationService;

        public MyBackgroundService(ILogger<MyBackgroundService> logger, ISendNotificationService sendNotificationService)
        {
            _logger = logger;
            _sendNotificationService = sendNotificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Fetching notification list at {time}", DateTime.UtcNow);
                await _sendNotificationService.NotifyCustomersAsync();

                // Delay for a period (e.g., 1 hour)
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
