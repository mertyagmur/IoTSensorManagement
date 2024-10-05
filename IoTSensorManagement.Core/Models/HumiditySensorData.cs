using IoTSensorManagement.Core.Interfaces;
using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.Models
{
	public class HumiditySensorData : ISensorData
	{

		[JsonPropertyName("hum")]
		public double Humidity { get; set; }

		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonIgnore]
		public SensorType SensorType => SensorType.Humidity;
	}
}
