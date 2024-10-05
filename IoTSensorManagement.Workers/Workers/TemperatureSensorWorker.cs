using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Workers.Base;

namespace IoTSensorManagement.Workers.Workers
{
	public class TemperatureSensorWorker : SensorWorker
	{
		public TemperatureSensorWorker(ISensor sensor, IApiClient apiClient, ILogger logger, SensorConfiguration config) : base(sensor, apiClient, logger, config) { }
	}
}
