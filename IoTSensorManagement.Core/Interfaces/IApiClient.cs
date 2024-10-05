namespace IoTSensorManagement.Core.Interfaces
{
	public interface IApiClient
	{
		Task SendDataAsync(string deviceId, IEnumerable<ISensorData> data);
	}
}
