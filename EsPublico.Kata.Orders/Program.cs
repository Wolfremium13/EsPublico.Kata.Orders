using EsPublico.Kata.Orders.Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// Configuration
var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ConfigFiles");
var configuration = new ConfigurationBuilder()
    .SetBasePath(configFilePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();
var databaseSettings = new DatabaseSettings();
configuration.GetSection("DatabaseSettings").Bind(databaseSettings);
var apiSettings = new ApiSettings();
configuration.GetSection("ApiSettings").Bind(apiSettings);

// Logging
var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Application started");

logger.LogInformation("Application stopped");