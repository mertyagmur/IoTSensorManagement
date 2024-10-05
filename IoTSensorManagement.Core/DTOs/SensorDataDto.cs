using System.Text.Json.Serialization;

namespace IoTSensorManagement.Api.DTOs
{
	public class SensorDataDto
	{
		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonPropertyName("illum")]
		public double? Illuminance { get; set; }

		[JsonPropertyName("temp")]
		public double? Temperature { get; set; }

		[JsonPropertyName("hum")]
		public double? Humidity { get; set; }

	}
}
