using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.DTOs
{
	public class HumiditySensorDataDto
	{
		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonPropertyName("hum")]
		public double Humidity { get; set; }
	}
}
