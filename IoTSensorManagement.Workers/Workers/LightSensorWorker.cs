using IoTSensorManagement.Workers.Base;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Workers.Workers
{
    public class LightSensorWorker : SensorWorker
    {
        public LightSensorWorker(ISensor sensor, IApiClient apiClient, ILogger logger, SensorConfiguration config) : base(sensor, apiClient, logger, config) { }
    }
}
