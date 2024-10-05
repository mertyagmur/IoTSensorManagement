using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using System.Collections.Concurrent;

namespace IoTSensorManagement.Sensors.Base
{
	public abstract class BaseSensor : ISensor
	{
		private readonly ConcurrentQueue<ISensorData> _dataBuffer = new ConcurrentQueue<ISensorData>();

		public string DeviceId { get; }
		public abstract SensorType Type { get; }
		public TimeSpan ReadingInterval { get; } = TimeSpan.FromMinutes(15);
		public TimeSpan ReportingInterval { get; } = TimeSpan.FromHours(1);

		protected BaseSensor(string deviceId, TimeSpan readingInterval, TimeSpan reportingInterval)
		{
			DeviceId = deviceId;
			ReadingInterval = readingInterval;
			ReportingInterval = reportingInterval;
		}

		public abstract Task<ISensorData> GenerateDataAsync();

		protected void BufferReading(ISensorData data)
		{
			_dataBuffer.Enqueue(data);
		}

		public Task<IReadOnlyCollection<ISensorData>> GetBufferedDataAsync()
		{
			return Task.FromResult<IReadOnlyCollection<ISensorData>>(_dataBuffer.ToArray());
		}

		public void ClearBuffer()
		{
			while (_dataBuffer.TryDequeue(out _)) { }
		}
	}
}
