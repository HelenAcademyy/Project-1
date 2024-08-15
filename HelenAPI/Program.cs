using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helen.Domain.Invites.Response;
using Helen.Repository;
using Helen.Service;
using Helen.Service.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddTransient<IHttpService, HttpService>();
builder.Services.AddTransient<IUtility, Utility>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IInviteService, InviteService>();
builder.Services.AddTransient<IAlertService, AlertService>();
builder.Services.AddTransient<IPaystackService, PaystackService>();
builder.Services.AddTransient<IMTNService, MTNService>();

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();

// Retrieve the connection string from configuration
string connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<HelenDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Health Checks with connection string from configuration
//builder.Services.AddHealthChecks()
//    .AddSqlServer(connectionString); // Add any other health checks you need

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();

app.MapControllers();

// Map health check endpoints
//app.MapHealthChecks("/health");

app.Run();
