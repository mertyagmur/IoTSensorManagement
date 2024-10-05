namespace IoTSensorManagement.Core.Models
{
	public class Device
	{
		public string Id { get; set; }
		public SensorType Type { get; set; }
		public TimeSpan ReadingInterval { get; set; }
		public TimeSpan ReportingInterval { get; set; }
		public virtual ICollection<SensorData> SensorData { get; set; }
	}
}
