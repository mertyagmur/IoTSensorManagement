using IoTSensorManagement.Core.Interfaces;
using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.Models
{
	public class TemperatureSensorData : ISensorData
	{

		[JsonPropertyName("temp")]
		public double Temperature { get; set; }

		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonIgnore]
		public SensorType SensorType => SensorType.Temperature;
	}
}
