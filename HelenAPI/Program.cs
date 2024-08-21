using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.Invites.Response;
using Helen.Repository;
using Helen.Service;
using Helen.Service.Utility;
using Microsoft.EntityFrameworkCore;
using static EmailService;

var builder = WebApplication.CreateBuilder(args);

// Retrieve configuration
var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddControllers();

// Add Swagger for API documentation and exploration in development environment
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Register services
builder.Services.AddTransient<IHttpService, HttpService>();
builder.Services.AddTransient<IUtility, Utility>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IInviteService, InviteService>();
builder.Services.AddTransient<IAlertService, AlertService>();
//builder.Services.AddTransient<IPaystackService, PaystackService>();
//builder.Services.AddTransient<IMTNService, MTNService>();
builder.Services.AddTransient<IBulkSmsService, BulkSmsService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Register other services
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();

// Configure database context
string connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HelenDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register health checks (uncomment if needed)
// builder.Services.AddHealthChecks()
//     .AddSqlServer(connectionString); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();

// Map controllers
app.MapControllers();

// Map health check endpoints (uncomment if needed)
// app.MapHealthChecks("/health");

app.Run();
