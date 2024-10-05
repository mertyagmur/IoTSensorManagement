using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Sensors.Base;

namespace IoTSensorManagement.Sensors.Sensors
{
	public class TemperatureSensor : BaseSensor
    {
        public override SensorType Type => SensorType.Temperature;
        private const double MinTemperature = -40.0;
        private const double MaxTemperature = 85.0;
        private const double Resolution = 0.1;
        private readonly Random _random = new Random();

        public TemperatureSensor(string deviceId, TimeSpan readingInterval, TimeSpan reportingInterval)
            : base(deviceId, readingInterval, reportingInterval) { }

        public override async Task<ISensorData> GenerateDataAsync()
        {
            await Task.Delay(100);
            var data = new TemperatureSensorData
            {
                Temperature = GetSimulatedTemperature(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
			BufferReading(data);
			return data;
		}

        private double GetSimulatedTemperature()
        {
            var now = DateTime.UtcNow;
            var hourOfDay = now.Hour + now.Minute / 60.0;

            var dailyProgress = Math.Sin((hourOfDay - 6) * Math.PI / 12);
            var baseTemperature = (MinTemperature + MaxTemperature) / 2;
            var amplitude = (MaxTemperature - MinTemperature) / 2;

            var temperature = baseTemperature + amplitude * dailyProgress;

            temperature += _random.NextDouble() * 2 - 1;

            return Math.Round(temperature / Resolution) * Resolution;
        }
    }
}
