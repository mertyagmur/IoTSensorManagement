using IoTSensorManagement.Api.DTOs;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace IoTSensorManagement.Core.Services
{
	public class ApiClient : IApiClient
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<ApiClient> _logger;

		public ApiClient(HttpClient httpClient, ILogger<ApiClient> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task SendDataAsync(string deviceId, IEnumerable<ISensorData> data)
		{
			try
			{
				var sensorDataDtos = ConvertToSensorDataDtos(data);
				var json = JsonSerializer.Serialize(sensorDataDtos);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync($"devices/{deviceId}/telemetry", content);
				response.EnsureSuccessStatusCode();
				_logger.LogInformation($"Successfully sent {sensorDataDtos.Count} readings for device {deviceId}");
			}
			catch (HttpRequestException ex)
			{
				_logger.LogError(ex, $"Error sending data for device {deviceId}. StatusCode: {ex.StatusCode}");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Unexpected error sending data for device {deviceId}");
				throw;
			}
		}

		private List<SensorDataDto> ConvertToSensorDataDtos(IEnumerable<ISensorData> sensorData)
		{
			var dtos = new List<SensorDataDto>();
			foreach (var data in sensorData)
			{
				var dto = new SensorDataDto
				{
					Timestamp = data.Timestamp
				};

				switch (data)
				{
					case LightSensorData lightData:
						dto.Illuminance = lightData.Illuminance;
						break;
					case TemperatureSensorData tempData:
						dto.Temperature = tempData.Temperature;
						break;
					case HumiditySensorData humidityData:
						dto.Humidity = humidityData.Humidity;
						break;
					default:
						_logger.LogWarning($"Unknown sensor data type: {data.GetType().Name}");
						continue;
				}
				dtos.Add(dto);
			}
			return dtos;
		}
	}
}

