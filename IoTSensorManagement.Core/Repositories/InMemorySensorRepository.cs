using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using System.Collections.Concurrent;

namespace IoTSensorManagement.Core.Repositories
{
	public class InMemorySensorRepository : ISensorRepository
	{
		private readonly ConcurrentDictionary<string, List<SensorData>> _sensorReadings = new ConcurrentDictionary<string, List<SensorData>>();
		private readonly ConcurrentDictionary<string, Device> _devices = new ConcurrentDictionary<string, Device>();

		public InMemorySensorRepository(IEnumerable<Device> initialDevices = null)
		{
			if (initialDevices != null)
			{
				foreach (var device in initialDevices)
				{
					_devices[device.Id] = device;
				}
			}
		}

		public Task AddSensorReadingAsync(string deviceId, ISensorData reading)
		{
			if (!_sensorReadings.TryGetValue(deviceId, out var readings))
			{
				readings = new List<SensorData>();
				_sensorReadings[deviceId] = readings;
			}

			var sensorData = new SensorData
			{
				DeviceId = deviceId,
				Timestamp = reading.Timestamp,
				SensorType = reading.SensorType,
				Value = GetSensorValue(reading)
			};

			readings.Add(sensorData);
			return Task.CompletedTask;
		}

		public Task<IEnumerable<ISensorData>> GetSensorReadingsAsync(string deviceId)
		{
			if (_sensorReadings.TryGetValue(deviceId, out var readings))
			{
				var result = readings
					.OrderByDescending(r => r.Timestamp)
					.Take(100)
					.Select(CreateSensorData)
					.ToList();

				return Task.FromResult((IEnumerable<ISensorData>)result);
			}
			return Task.FromResult(Enumerable.Empty<ISensorData>());
		}

		public Task<IEnumerable<DailyMaximumReading>> GetDailyMaximumReadingsAsync(string deviceId, int days)
		{
			if (!_sensorReadings.TryGetValue(deviceId, out var readings))
			{
				return Task.FromResult(Enumerable.Empty<DailyMaximumReading>());
			}

			var endDate = DateTime.UtcNow.Date;
			var startDate = endDate.AddDays(-days + 1);

			var dailyMaxReadings = readings
				.Where(r => {
					var date = DateTimeOffset.FromUnixTimeSeconds(r.Timestamp).DateTime.Date;
					return date >= startDate && date <= endDate;
				})
				.GroupBy(r => DateTimeOffset.FromUnixTimeSeconds(r.Timestamp).DateTime.Date)
				.Select(g => new DailyMaximumReading
				{
					Date = g.Key,
					MaxValue = g.Max(r => r.Value ?? 0)
				})
				.OrderBy(r => r.Date)
				.ToList();

			//Fill in any missing days with zero values
			var allDays = Enumerable.Range(0, days)
				.Select(offset => endDate.AddDays(-offset))
				.Reverse();

			var result = allDays.GroupJoin(
				dailyMaxReadings,
				day => day,
				reading => reading.Date,
				(day, readings) => readings.FirstOrDefault() ?? new DailyMaximumReading { Date = day, MaxValue = 0 }
			);

			return Task.FromResult(result);
		}

		public Task<Device> GetDeviceAsync(string deviceId)
		{
			if (_devices.TryGetValue(deviceId, out var device))
			{
				return Task.FromResult(device);
			}
			return Task.FromResult<Device>(null);
		}

		public Task AddOrUpdateDeviceAsync(Device device)
		{
			_devices[device.Id] = device;
			return Task.CompletedTask;
		}

		private double? GetSensorValue(ISensorData sensorData)
		{
			return sensorData switch
			{
				LightSensorData lightData => lightData.Illuminance,
				TemperatureSensorData tempData => tempData.Temperature,
				HumiditySensorData humData => humData.Humidity,
				_ => null
			};
		}

		private ISensorData CreateSensorData(SensorData data)
		{
			return data.SensorType switch
			{
				SensorType.Light => new LightSensorData { Illuminance = data.Value ?? 0, Timestamp = data.Timestamp },
				SensorType.Temperature => new TemperatureSensorData { Temperature = data.Value ?? 0, Timestamp = data.Timestamp },
				SensorType.Humidity => new HumiditySensorData { Humidity = data.Value ?? 0, Timestamp = data.Timestamp },
				_ => throw new ArgumentException($"Unknown sensor type: {data.SensorType}")
			};
		}
	}
}
