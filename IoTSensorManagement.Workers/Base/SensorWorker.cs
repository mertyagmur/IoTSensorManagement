using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Workers.Base
{
    public abstract class SensorWorker : BackgroundService
	{
		protected readonly ISensor Sensor;
		protected readonly IApiClient ApiClient;
		private readonly SensorConfiguration _config;
		protected readonly ILogger Logger;

		public SensorWorker(ISensor sensor, IApiClient apiClient, ILogger logger, SensorConfiguration config)
		{
			Console.WriteLine($"SensorWorker created for DeviceId: {sensor.DeviceId}");
			Sensor = sensor;
			ApiClient = apiClient;
			Logger = logger;
			_config = config;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var readTimer = new PeriodicTimer(Sensor.ReadingInterval);
			using var reportTimer = new PeriodicTimer(Sensor.ReportingInterval);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					await Task.WhenAny(
						ReadDataAsync(readTimer, stoppingToken),
						ReportDataAsync(reportTimer, stoppingToken)
					);
				}
				catch (Exception ex)
				{
					Logger.LogError(ex, $"Error in {Sensor.Type} sensor worker");
					await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
				}
			}
		}

		private async Task ReadDataAsync(PeriodicTimer timer, CancellationToken stoppingToken)
		{
			if (await timer.WaitForNextTickAsync(stoppingToken))
			{
				var data = await Sensor.GenerateDataAsync();
				Logger.LogInformation($"Read data from {Sensor.DeviceId} at: {data.Timestamp}");
			}
		}

		private async Task ReportDataAsync(PeriodicTimer timer, CancellationToken stoppingToken)
		{
			if (await timer.WaitForNextTickAsync(stoppingToken))
			{
				var bufferedData = await Sensor.GetBufferedDataAsync();
				if (bufferedData.Count > 0)
				{
					await ApiClient.SendDataAsync(Sensor.DeviceId, bufferedData);
					Logger.LogInformation($"Sent {bufferedData.Count} readings from {Sensor.DeviceId} sensor to API");
					Sensor.ClearBuffer();
				}
				else
				{
					Logger.LogInformation($"No data to send for {Sensor.DeviceId}");
				}
			}
		}
	}
}
