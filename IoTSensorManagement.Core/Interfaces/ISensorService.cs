using IoTSensorManagement.Api.DTOs;

namespace IoTSensorManagement.Core.Interfaces
{
	public interface ISensorService
	{
		Task ProcessTelemetryAsync(string deviceId, List<SensorDataDto> telemetryData);
		Task<List<Dictionary<string, object>>> GetAllDataAsync(string deviceId);
		Task<object> GetDeviceStatisticsAsync(string deviceId);
	}
}
