using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Sensors.Base;

namespace IoTSensorManagement.Sensors.Sensors
{
	public class HumiditySensor : BaseSensor
	{
		public override SensorType Type => SensorType.Humidity;
		private const double MinHumidity = 30.0;
		private const double MaxHumidity = 80.0;
		private const double Resolution = 0.1;

		private readonly Random _random = new Random();

		public HumiditySensor(string deviceId, TimeSpan readingInterval, TimeSpan reportingInterval)
			: base(deviceId, readingInterval, reportingInterval) { }

		public override async Task<ISensorData> GenerateDataAsync()
		{
			await Task.Delay(50); // Simulating sensor read time
			var data = new HumiditySensorData
			{
				Humidity = GetSimulatedHumidity(),
				Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
			};
			BufferReading(data);
			return data;
		}

		private double GetSimulatedHumidity()
		{
			var now = DateTime.UtcNow;
			var hourOfDay = now.Hour + now.Minute / 60.0;

			var dailyProgress = Math.Cos((hourOfDay - 4) * Math.PI / 12);
			var baseHumidity = (MinHumidity + MaxHumidity) / 2; 
			var amplitude = (MaxHumidity - MinHumidity) / 2;

			var humidity = baseHumidity + amplitude * dailyProgress;

			humidity += (_random.NextDouble() * 10 - 5);

			humidity = Math.Max(MinHumidity, Math.Min(humidity, MaxHumidity));

			return Math.Round(humidity / Resolution) * Resolution;
		}
	}
}
