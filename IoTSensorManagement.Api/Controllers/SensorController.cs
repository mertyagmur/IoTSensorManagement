using IoTSensorManagement.Api.DTOs;
using IoTSensorManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SensorConsumer.Controllers
{
	[ApiController]
	public class SensorController : ControllerBase
	{
		private readonly ISensorService _sensorService;
		private readonly ILogger<SensorController> _logger;

		public SensorController(ISensorService sensorService, ILogger<SensorController> logger)
		{
			_sensorService = sensorService;
			_logger = logger;
		}

		[HttpPost("devices/{deviceId}/telemetry")]
		public async Task<IActionResult> PostTelemetry(string deviceId, [FromBody] List<SensorDataDto> telemetryData)
		{
			_logger.LogInformation("Received telemetry data for device {DeviceId}. Data count: {Count}", deviceId, telemetryData.Count);

			if (!ModelState.IsValid)
			{
				_logger.LogWarning("Invalid model state for device {DeviceId}", deviceId);
				return BadRequest(ModelState);
			}

			try
			{
				await _sensorService.ProcessTelemetryAsync(deviceId, telemetryData);
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing telemetry data for device {DeviceId}", deviceId);
				return StatusCode(500, "An error occurred while processing the telemetry data.");
			}
		}

		[HttpGet("devices/{deviceId}")]
		public async Task<IActionResult> GetAllData(string deviceId)
		{
			_logger.LogInformation("Retrieving all data for device {DeviceId}", deviceId);

			try
			{
				var data = await _sensorService.GetAllDataAsync(deviceId);
				if (!data.Any())
				{
					_logger.LogWarning("No data found for device {DeviceId}", deviceId);
					return NotFound($"No data found for device ID {deviceId}");
				}

				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving data for device {DeviceId}", deviceId);
				return StatusCode(500, "An error occurred while retrieving the sensor data.");
			}
		}

		[HttpGet("devices/{deviceId}/statistics")]
		public async Task<IActionResult> GetStatistics(string deviceId)
		{
			_logger.LogInformation("Retrieving statistics for device {DeviceId}", deviceId);

			try
			{
				var statistics = await _sensorService.GetDeviceStatisticsAsync(deviceId);
				if (statistics == null)
				{
					_logger.LogWarning("No statistics data found for device {DeviceId}", deviceId);
					return NotFound($"No data found for device ID {deviceId}");
				}

				return Ok(statistics);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving statistics for device {DeviceId}", deviceId);
				return StatusCode(500, "An error occurred while retrieving the statistics.");
			}
		}
	}
}
