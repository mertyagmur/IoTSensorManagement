using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Shared.Interfaces
{
    public interface ISharedConfigurationProvider
    {
        IEnumerable<SensorConfiguration> GetSensorConfigurations();
    }
}
