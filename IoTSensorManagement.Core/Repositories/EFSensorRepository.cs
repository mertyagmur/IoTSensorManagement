using IoTSensorManagement.Core.Data;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace IoTSensorManagement.Core.Repositories
{
	public class EFSensorRepository : ISensorRepository
	{
		private readonly ApplicationDbContext _context;

		public EFSensorRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddSensorReadingAsync(string deviceId, ISensorData reading)
		{
			var sensorData = new SensorData
			{
				DeviceId = deviceId,
				Timestamp = reading.Timestamp,
				SensorType = reading.SensorType,
				Value = GetSensorValue(reading)
			};

			_context.SensorData.Add(sensorData);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<ISensorData>> GetSensorReadingsAsync(string deviceId)
		{
			return await _context.SensorData
				.Where(sd => sd.DeviceId == deviceId)
				.OrderByDescending(sd => sd.Timestamp)
				.Take(100)
				.Select(sd => CreateSensorData(sd))
				.ToListAsync();
		}

		public async Task<IEnumerable<DailyMaximumReading>> GetDailyMaximumReadingsAsync(string deviceId, int days)
		{
			var endDate = DateTime.UtcNow.Date;
			var startDate = endDate.AddDays(-days + 1);

			var sensorData = await _context.SensorData
				.Where(sd => sd.DeviceId == deviceId &&
							 sd.Timestamp >= new DateTimeOffset(startDate).ToUnixTimeSeconds() &&
							 sd.Timestamp < new DateTimeOffset(endDate.AddDays(1)).ToUnixTimeSeconds())
				.ToListAsync();  

			var dailyMaxReadings = sensorData
				.GroupBy(sd => DateTimeOffset.FromUnixTimeSeconds(sd.Timestamp).DateTime.Date)
				.Select(g => new DailyMaximumReading
				{
					Date = g.Key,
					MaxValue = g.Max(sd => sd.Value ?? 0)
				})
				.OrderBy(r => r.Date)
				.ToList();

			// Fill in any missing days with zero values
			var allDays = Enumerable.Range(0, days)
				.Select(offset => endDate.AddDays(-offset))
				.Reverse();

			var result = allDays.GroupJoin(
				dailyMaxReadings,
				day => day.Date,
				reading => reading.Date,
				(day, readings) => readings.FirstOrDefault() ?? new DailyMaximumReading { Date = day, MaxValue = 0 }
			);

			return result;
		}

		public async Task<Device> GetDeviceAsync(string deviceId)
		{
			return await _context.Devices.FindAsync(deviceId);
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

		private static ISensorData CreateSensorData(SensorData data)
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
