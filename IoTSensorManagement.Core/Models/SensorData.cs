namespace IoTSensorManagement.Core.Models
{
	public class SensorData
	{
		public int Id { get; set; }
		public string DeviceId { get; set; }
		public long Timestamp { get; set; }
		public SensorType SensorType { get; set; }
		public double? Value { get; set; }
	}
}
