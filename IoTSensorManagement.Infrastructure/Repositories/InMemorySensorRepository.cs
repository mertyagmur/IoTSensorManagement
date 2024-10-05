using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTSensorManagement.Core.Repositories
{
	public class InMemorySensorRepository : ISensorRepository
	{
		private readonly Dictionary<string, List<ISensorData>> _storage = new();

		public void SaveReading(string deviceId, ISensorData data)
		{
			if (!_storage.TryGetValue(deviceId, out var readings))
			{
				readings = new List<ISensorData>();
				_storage[deviceId] = readings;
			}
			readings.Add(data);
		}

		public IEnumerable<ISensorData> GetAllReadings(string deviceId)
		{
			return _storage.TryGetValue(deviceId, out var readings)
				? readings
				: Enumerable.Empty<ISensorData>();
		}

		public IEnumerable<DailyMaximumReading> GetDailyMaximumReadings(string deviceId)
		{
			if (!_storage.TryGetValue(deviceId, out var readings))
			{
				return Enumerable.Empty<DailyMaximumReading>();
			}

			return readings
				.GroupBy(r => DateTimeOffset.FromUnixTimeMilliseconds(r.Time).Date)
				.Select(g => new DailyMaximumReading
				{
					Date = g.Key,
					MaxValue = g.Max(r => r.Value)
				})
				.OrderBy(d => d.Date);
		}
	}
}
