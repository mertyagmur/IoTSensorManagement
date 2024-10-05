using System.Text.Json.Serialization;
using IoTSensorManagement.Core.Interfaces;

namespace IoTSensorManagement.Core.Models
{
	public class LightSensorData : ISensorData
	{

		[JsonPropertyName("illum")]
		public double Illuminance { get; set; }

		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonIgnore]
		public SensorType SensorType => SensorType.Light;
	}
}
