using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.DTOs
{
	public class TemperatureSensorDataDto
	{
		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonPropertyName("temp")]
		public double Temperature { get; set; }
	}
}
