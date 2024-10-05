using IoTSensorManagement.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IoTSensorManagement.Shared.Converters
{
    public class SensorTypeJsonConverter : JsonConverter<SensorType>
    {
        public override SensorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string stringValue = reader.GetString();
            return Enum.Parse<SensorType>(stringValue, true);
        }

        public override void Write(Utf8JsonWriter writer, SensorType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
