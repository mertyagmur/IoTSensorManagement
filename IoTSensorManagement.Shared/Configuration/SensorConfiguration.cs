using IoTSensorManagement.Core.Models;

namespace IoTSensorManagement.Shared.Configuration
{
    public class SensorConfiguration
    {
        public string DeviceId { get; set; }
        public SensorType Type { get; set; }
        public TimeSpan ReadingInterval { get; set; }
        public TimeSpan ReportingInterval { get; set; }
    }
}
