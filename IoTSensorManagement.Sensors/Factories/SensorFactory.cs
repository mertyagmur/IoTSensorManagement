using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Sensors.Interfaces;
using IoTSensorManagement.Sensors.Sensors;
using IoTSensorManagement.Shared.Configuration;

namespace IoTSensorManagement.Sensors.Factories
{
    public class SensorFactory : ISensorFactory
	{
		public ISensor CreateSensor(SensorConfiguration config)
		{
			return config.Type switch
			{
				SensorType.Light => new LightSensor(config.DeviceId, config.ReadingInterval, config.ReportingInterval),
				SensorType.Temperature => new TemperatureSensor(config.DeviceId, config.ReadingInterval, config.ReportingInterval),
				SensorType.Humidity => new HumiditySensor(config.DeviceId, config.ReadingInterval, config.ReportingInterval),
				_ => throw new ArgumentException($"Unknown sensor type: {config.Type}")
			};
		}
	}
}
