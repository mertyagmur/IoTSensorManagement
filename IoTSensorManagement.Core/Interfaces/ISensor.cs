using IoTSensorManagement.Core.Models;

namespace IoTSensorManagement.Core.Interfaces
{
	public interface ISensor
	{
		string DeviceId { get; }
		SensorType Type { get; }
		Task<ISensorData> GenerateDataAsync();
		TimeSpan ReadingInterval { get; }
		TimeSpan ReportingInterval { get; }
		Task<IReadOnlyCollection<ISensorData>> GetBufferedDataAsync();
		void ClearBuffer();
	}
}
