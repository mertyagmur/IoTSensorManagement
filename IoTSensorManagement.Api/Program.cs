using IoTSensorManagement.Core.Data;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Mappings;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Core.Repositories;
using IoTSensorManagement.Core.Services;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var sharedConfigPath = builder.Configuration["SharedConfigPath"];
builder.Services.AddSingleton<ISharedConfigurationProvider>(sp =>
{
	var logger = sp.GetRequiredService<ILogger<FileBasedConfigurationProvider>>();
	return new FileBasedConfigurationProvider(sharedConfigPath, logger);
});

var useInMemoryDb = builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

if (useInMemoryDb)
{
	builder.Services.AddSingleton<ISensorRepository, InMemorySensorRepository>();
}
else
{
	builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
	builder.Services.AddScoped<ISensorRepository, EFSensorRepository>();
}

builder.Services.AddScoped<ISensorService, SensorService>();


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var logger = services.GetRequiredService<ILogger<Program>>();

	try
	{
		var configProvider = services.GetRequiredService<ISharedConfigurationProvider>();
		var sensorConfigs = configProvider.GetSensorConfigurations();

		if (!useInMemoryDb)
		{
			var context = services.GetRequiredService<ApplicationDbContext>();

			context.Database.EnsureCreated();

			foreach (var config in sensorConfigs)
			{
				var device = await context.Devices.FindAsync(config.DeviceId);
				if (device == null)
				{
					device = new Device
					{
						Id = config.DeviceId,
						Type = config.Type,
						ReadingInterval = config.ReadingInterval,
						ReportingInterval = config.ReportingInterval
					};
					context.Devices.Add(device);
				}
				else
				{
					device.Type = config.Type;
					device.ReadingInterval = config.ReadingInterval;
					device.ReportingInterval = config.ReportingInterval;
					context.Devices.Update(device);
				}
			}

			await context.SaveChangesAsync();
			logger.LogInformation("Database initialized with device configurations.");
		}
		else
		{
			var repository = services.GetRequiredService<ISensorRepository>();
			if (repository is InMemorySensorRepository inMemoryRepo)
			{
				foreach (var config in sensorConfigs)
				{
					await inMemoryRepo.AddOrUpdateDeviceAsync(new Device
					{
						Id = config.DeviceId,
						Type = config.Type,
						ReadingInterval = config.ReadingInterval,
						ReportingInterval = config.ReportingInterval
					});
				}
				logger.LogInformation("In-memory repository initialized with device configurations.");
			}
		}
	}
	catch (Exception ex)
	{
		logger.LogError(ex, "An error occurred while initializing the database.");
	}
}

app.Run();
