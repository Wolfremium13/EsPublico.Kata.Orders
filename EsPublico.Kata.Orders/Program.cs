using EsPublico.Kata.Orders.Application;
using EsPublico.Kata.Orders.Infrastructure.Apis;
using EsPublico.Kata.Orders.Infrastructure.Config;
using EsPublico.Kata.Orders.Infrastructure.Databases;
using EsPublico.Kata.Orders.Infrastructure.Generators;
using EsPublico.Kata.Orders.Infrastructure.Repositories;
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
var programSettings = new ProgramSettings();
configuration.GetSection("ProgramSettings").Bind(programSettings);

// Logging
var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
var logger = loggerFactory.CreateLogger<Program>();

// Injection
logger.LogInformation("Application starting...");

var postgresAdapter = new PostgresAdapter(databaseSettings);
var ordersRepository =
    new PostgresOrdersRepository(postgresAdapter, loggerFactory.CreateLogger<PostgresOrdersRepository>());
var dateTimeGenerator = new DateTimeGenerator();
var localFileSystemRepository = new LocalFilesRepository(
    dateTimeGenerator,
    loggerFactory.CreateLogger<LocalFilesRepository>());
var httpClientsFactory = new HttpClientFactory();
var apiOrders = new OrdersHttpApi(httpClientsFactory, apiSettings, loggerFactory.CreateLogger<OrdersHttpApi>());
var ordersService = new OrdersService(
    ordersRepository,
    localFileSystemRepository,
    apiOrders,
    dateTimeGenerator,
    loggerFactory.CreateLogger<OrdersService>());
var summaryRepository = new PostgresSummaryRepository(
    postgresAdapter);
var summaryService = new SummaryService(
    summaryRepository,
    localFileSystemRepository,
    loggerFactory.CreateLogger<SummaryService>());

// Application
logger.LogInformation("Application started.");
if (programSettings.Ingest == true)
{
    await ordersService.Ingest();
}

if (programSettings.Summary == true)
{
    await summaryService.Summary();
}

logger.LogInformation("Application finished.");