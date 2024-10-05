using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Services;
using IoTSensorManagement.Sensors.Factories;
using IoTSensorManagement.Sensors.Interfaces;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Shared.Interfaces;
using IoTSensorManagement.Workers;
using IoTSensorManagement.Workers.Factories;
using IoTSensorManagement.Workers.Interfaces;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
	var baseUrl = builder.Configuration["ApiBaseUrl"];
	client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddSingleton<IWorkerFactory, WorkerFactory>();
builder.Services.AddSingleton<ISensorFactory, SensorFactory>();

var sharedConfigPath = builder.Configuration["SharedConfigPath"];
builder.Services.AddSingleton<ISharedConfigurationProvider>(sp =>
{
	var logger = sp.GetRequiredService<ILogger<FileBasedConfigurationProvider>>();
	return new FileBasedConfigurationProvider(sharedConfigPath, logger);
});

builder.Services.AddHostedService<WorkerManager>();

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var host = builder.Build();
host.Run();
