using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Azure.StackExchangeRedis;
using StackExchange.Redis;
using System.Diagnostics;
using System;
using StackExchange.Redis.Maintenance;
using System.Security.Principal;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Check the environment variable for the Redis cache host name
var cacheHostName = Environment.GetEnvironmentVariable("AZURE_REDIS_HOST");
if (string.IsNullOrEmpty(cacheHostName))
{
    throw new InvalidOperationException("The environment variable 'AZURE_REDIS_HOST' is not set.");
}

var configurationOptions = ConfigurationOptions.Parse($"{cacheHostName}:6380");
var redisConnectionString = $"{cacheHostName}:6380";

// Uncomment the following lines corresponding to the authentication type you want to use.
// For system-assigned identity.
// In the Azure Portal, we need to set up Redis service to grant Contributor access to the system-assigned identity
// for the container app that hosts this Web API service.
await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

// For user-assigned identity.
// var managedIdentityClientId = Environment.GetEnvironmentVariable("AZURE_REDIS_CLIENTID");
// await configurationOptions.ConfigureForAzureWithUserAssignedManagedIdentityAsync(managedIdentityClientId);

// Service principal secret.
// var clientId = Environment.GetEnvironmentVariable("AZURE_REDIS_CLIENTID");
// var tenantId = Environment.GetEnvironmentVariable("AZURE_REDIS_TENANTID");
// var secret = Environment.GetEnvironmentVariable("AZURE_REDIS_CLIENTSECRET");
// await configurationOptions.ConfigureForAzureWithServicePrincipalAsync(clientId, tenantId, secret);

var connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(configurationOptions);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConfigurationOptions = configurationOptions;
    //options.Configuration = redisConnectionString;
    options.InstanceName = "SampleInstance";
});

// Uncomment the following line if you need to use the ConnectionMultiplexer directly
// (for example, for advanced Redis operations like Pub/Sub or working with Redis data structures).
// builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//     ConnectionMultiplexer.Connect(configurationOptions));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
