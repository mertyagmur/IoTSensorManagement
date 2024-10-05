using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Sensors.Interfaces;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Workers.Base;
using IoTSensorManagement.Workers.Interfaces;
using IoTSensorManagement.Workers.Workers;

namespace IoTSensorManagement.Workers.Factories
{
    public class WorkerFactory : IWorkerFactory
	{
		private readonly ISensorFactory _sensorFactory;
		private readonly IApiClient _apiClient;
		private readonly ILogger<SensorWorker> _logger;

		public WorkerFactory(ISensorFactory sensorFactory, IApiClient apiClient, ILogger<SensorWorker> logger)
		{
			_sensorFactory = sensorFactory;
			_apiClient = apiClient;
			_logger = logger;
		}

		public IHostedService CreateWorker(SensorConfiguration config)
		{
			var sensor = _sensorFactory.CreateSensor(config);
			return config.Type switch
			{
				SensorType.Light => new LightSensorWorker(sensor, _apiClient, _logger, config),
				SensorType.Temperature => new TemperatureSensorWorker(sensor, _apiClient, _logger, config),
				SensorType.Humidity => new HumiditySensorWorker(sensor, _apiClient, _logger, config),
				_ => throw new ArgumentException($"Unknown sensor type: {config.Type}")
			};
		}
	}
}
