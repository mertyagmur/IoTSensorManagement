using AutoMapper;
using IoTSensorManagement.Api.DTOs;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using Microsoft.Extensions.Logging;

namespace IoTSensorManagement.Core.Services
{
	public class SensorService : ISensorService
	{
		private readonly ISensorRepository _repository;
		private readonly IMapper _mapper;
		private readonly ILogger<SensorService> _logger;

		public SensorService(ISensorRepository repository, IMapper mapper, ILogger<SensorService> logger)
		{
			_repository = repository;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task ProcessTelemetryAsync(string deviceId, List<SensorDataDto> telemetryData)
		{
			foreach (var data in telemetryData)
			{
				ISensorData sensorData = MapToSensorData(data);

				await _repository.AddSensorReadingAsync(deviceId, sensorData);

				_logger.LogDebug("Added sensor reading for device {DeviceId}, timestamp {Timestamp}",
						deviceId, sensorData.Timestamp);
			}

			_logger.LogInformation("Successfully processed telemetry data for device {DeviceId}", deviceId);
		}

		public async Task<List<Dictionary<string, object>>> GetAllDataAsync(string deviceId)
		{
			var readings = await _repository.GetSensorReadingsAsync(deviceId);
			if (!readings.Any())
			{
				return new List<Dictionary<string, object>>();
			}

			var result = readings.Select(reading =>
			{
				var sensorData = new Dictionary<string, object>
		{
			{ "time", reading.Timestamp }
        };

				switch (reading.SensorType)
				{
					case SensorType.Light:
						sensorData["illum"] = ((LightSensorData)reading).Illuminance;
						break;
					case SensorType.Temperature:
						sensorData["temp"] = ((TemperatureSensorData)reading).Temperature;
						break;
					case SensorType.Humidity:
						sensorData["hum"] = ((HumiditySensorData)reading).Humidity;
						break;
					default:
						return null;
				}

				return sensorData;
			})
			.Where(data => data != null)
			.ToList();

			return result;
		}

		public async Task<object> GetDeviceStatisticsAsync(string deviceId)
		{
			var device = await _repository.GetDeviceAsync(deviceId);
			if (device == null)
			{
				return null;
			}

			var maxReadings = await _repository.GetDailyMaximumReadingsAsync(deviceId, 30);
			if (!maxReadings.Any())
			{
				return null;
			}

			var result = new List<object>();

			foreach (var r in maxReadings)
			{
				var dateString = r.Date.ToString("yyyy-MM-dd");
				object responseItem;

				switch (device.Type)
				{
					case SensorType.Light:
						responseItem = new
						{
							date = dateString,
							maxIlluminance = r.MaxValue
						};
						break;
					case SensorType.Temperature:
						responseItem = new
						{
							date = dateString,
							maxTemperature = r.MaxValue 
						};
						break;
					case SensorType.Humidity:
						responseItem = new
						{
							date = dateString,
							maxHumidity = r.MaxValue
						};
						break;
					default:
						responseItem = new
						{
							date = dateString,
							maxValue = r.MaxValue 
						};
						break;
				}

				result.Add(responseItem);
			}

			return result;
		}

		private ISensorData MapToSensorData(SensorDataDto data)
		{
			if (data.Illuminance.HasValue)
				return _mapper.Map<LightSensorData>(data);
			if (data.Temperature.HasValue)
				return _mapper.Map<TemperatureSensorData>(data);
			if (data.Humidity.HasValue)
				return _mapper.Map<HumiditySensorData>(data);

			throw new ArgumentException("Unable to determine sensor type from data");
		}

		private object GetTypedMaxValue(SensorType sensorType, double maxValue)
		{
			return sensorType switch
			{
				SensorType.Light => new { maxIlluminance = maxValue },
				SensorType.Temperature => new { maxTemperature = maxValue },
				SensorType.Humidity => new { maxHumidity = maxValue },
				_ => throw new ArgumentException($"Unknown sensor type: {sensorType}")
			};
		}
	}
}
