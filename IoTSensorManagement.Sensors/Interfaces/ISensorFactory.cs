using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Sensors.Interfaces
{
    public interface ISensorFactory
    {
        ISensor CreateSensor(SensorConfiguration config);
    }
}
