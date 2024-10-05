using IoTSensorManagement.Core.Models;

namespace IoTSensorManagement.Core.Interfaces
{
	public interface ISensorRepository
	{
		Task AddSensorReadingAsync(string deviceId, ISensorData reading);
		Task<IEnumerable<ISensorData>> GetSensorReadingsAsync(string deviceId);
		Task<IEnumerable<DailyMaximumReading>> GetDailyMaximumReadingsAsync(string deviceId, int days);
		Task<Device> GetDeviceAsync(string deviceId);
	}
}
