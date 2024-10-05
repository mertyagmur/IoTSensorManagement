using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Workers.Base;

namespace IoTSensorManagement.Workers.Workers
{
	public class HumiditySensorWorker : SensorWorker
	{
		public HumiditySensorWorker(ISensor sensor, IApiClient apiClient, ILogger logger, SensorConfiguration config) : base(sensor, apiClient, logger, config) { }
	}
}
