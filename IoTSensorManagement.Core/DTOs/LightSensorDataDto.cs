using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.DTOs
{
	public class LightSensorDataDto
	{
		[JsonPropertyName("time")]
		public long Timestamp { get; set; }

		[JsonPropertyName("illum")]
		public double Illuminance { get; set; }
	}
}
