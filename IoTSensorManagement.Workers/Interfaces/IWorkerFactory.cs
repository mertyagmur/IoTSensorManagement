using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Workers.Interfaces
{
    public interface IWorkerFactory
    {
        IHostedService CreateWorker(SensorConfiguration config);
    }
}
