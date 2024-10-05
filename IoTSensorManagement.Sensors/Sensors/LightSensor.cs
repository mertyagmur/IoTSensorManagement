using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Sensors.Base;

namespace IoTSensorManagement.Sensors.Sensors
{
    public class LightSensor : BaseSensor
    {
        public override SensorType Type => SensorType.Light;
        private const double MaxIlluminance = 120000.0; // Maximum illuminance on a clear sunny day
        private const double Resolution = 0.5;

        public LightSensor(string deviceId, TimeSpan readingInterval, TimeSpan reportingInterval)
            : base(deviceId, readingInterval, reportingInterval) { }

        public override async Task<ISensorData> GenerateDataAsync()
        {
            await Task.Delay(100); // Simulating sensor read time
            var data = new LightSensorData
            {
                Illuminance = GetSimulatedIlluminance(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
            BufferReading(data);
            return data;
        }

        private double GetSimulatedIlluminance()
        {
            var now = DateTime.UtcNow;
            var dayProgress = now.TimeOfDay.TotalMinutes % 720 / 720.0;
            var baseIlluminance = CalculateBaseIlluminance(dayProgress);

            // Add some daily variation
            var dailyVariation = Math.Sin(now.DayOfYear * Math.PI / 180) * MaxIlluminance * 0.1;

            var illuminance = Math.Max(0, Math.Min(baseIlluminance + dailyVariation, MaxIlluminance));
            return Math.Round(illuminance / Resolution) * Resolution;
        }

        private double CalculateBaseIlluminance(double dayProgress)
        {
            if (dayProgress <= 0.5)
            {
                return dayProgress * 2 * MaxIlluminance;
            }
            else
            {
                return (1 - (dayProgress - 0.5) * 2) * MaxIlluminance;
            }
        }
    }
}
