using IoTSensorManagement.Core.Models;
using System.Text.Json.Serialization;

namespace IoTSensorManagement.Core.Interfaces
{
	public interface ISensorData
	{
		[JsonPropertyName("time")]
		long Timestamp { get; set; }
		SensorType SensorType { get; }
	}
}
