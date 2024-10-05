using AutoMapper;
using IoTSensorManagement.Api.DTOs;
using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Core.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace IoTSensorManagement.Tests.Services
{
	public class SensorServiceTests
	{
		private readonly Mock<ISensorRepository> _mockRepository;
		private readonly Mock<IMapper> _mockMapper;
		private readonly Mock<ILogger<SensorService>> _mockLogger;
		private readonly SensorService _sensorService;

		public SensorServiceTests()
		{
			_mockRepository = new Mock<ISensorRepository>();
			_mockMapper = new Mock<IMapper>();
			_mockLogger = new Mock<ILogger<SensorService>>();
			_sensorService = new SensorService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task ProcessTelemetryAsync_ValidData_AddsReadingsToRepository()
		{
			// Arrange
			var deviceId = "TestDevice";
			var telemetryData = new List<SensorDataDto>
			{
				new SensorDataDto { Timestamp = 1633046400, Illuminance = 100000 },
				new SensorDataDto { Timestamp = 1633050000, Humidity = 60.0 }
			};

			_mockMapper.Setup(m => m.Map<LightSensorData>(It.IsAny<SensorDataDto>()))
				.Returns((SensorDataDto dto) => new LightSensorData { Timestamp = dto.Timestamp, Illuminance = dto.Illuminance.Value });

			_mockMapper.Setup(m => m.Map<HumiditySensorData>(It.IsAny<SensorDataDto>()))
				.Returns((SensorDataDto dto) => new HumiditySensorData { Timestamp = dto.Timestamp, Humidity = dto.Humidity.Value });

			// Act
			await _sensorService.ProcessTelemetryAsync(deviceId, telemetryData);

			// Assert
			_mockRepository.Verify(r => r.AddSensorReadingAsync(deviceId, It.IsAny<ISensorData>()), Times.Exactly(2));
		}

		[Fact]
		public async Task ProcessTelemetryAsync_EmptyData_DoesNotAddReadings()
		{
			// Arrange
			var deviceId = "TestDevice";
			var telemetryData = new List<SensorDataDto>();

			// Act
			await _sensorService.ProcessTelemetryAsync(deviceId, telemetryData);

			// Assert
			_mockRepository.Verify(r => r.AddSensorReadingAsync(It.IsAny<string>(), It.IsAny<ISensorData>()), Times.Never);
		}

		[Fact]
		public async Task ProcessTelemetryAsync_NullData_ThrowsNullReferenceException()
		{
			// Arrange
			var deviceId = "TestDevice";

			// Act & Assert
			await Assert.ThrowsAsync<NullReferenceException>(() => _sensorService.ProcessTelemetryAsync(deviceId, null));
		}

		[Fact]
		public async Task GetAllDataAsync_ReturnsFormattedData()
		{
			// Arrange
			var deviceId = "TestDevice";
			var mockReadings = new List<ISensorData>
			{
				new TemperatureSensorData { Timestamp = 1633046400, Temperature = 25.5 },
				new HumiditySensorData { Timestamp = 1633050000, Humidity = 60.0 }
			};

			_mockRepository.Setup(r => r.GetSensorReadingsAsync(deviceId))
				.ReturnsAsync(mockReadings);

			// Act
			var result = await _sensorService.GetAllDataAsync(deviceId);

			// Assert
			Assert.Equal(2, result.Count);
			Assert.Equal(25.5, result[0]["temp"]);
			Assert.Equal(60.0, result[1]["hum"]);
		}

		[Fact]
		public async Task GetAllDataAsync_NoData_ReturnsEmptyList()
		{
			// Arrange
			var deviceId = "TestDevice";
			_mockRepository.Setup(r => r.GetSensorReadingsAsync(deviceId))
				.ReturnsAsync(new List<ISensorData>());

			// Act
			var result = await _sensorService.GetAllDataAsync(deviceId);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetDeviceStatisticsAsync_NoDevice_ReturnsNull()
		{
			// Arrange
			var deviceId = "NonexistentDevice";
			_mockRepository.Setup(r => r.GetDeviceAsync(deviceId))
				.ReturnsAsync((Device)null);

			// Act
			var result = await _sensorService.GetDeviceStatisticsAsync(deviceId);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task GetDeviceStatisticsAsync_NoReadings_ReturnsNull()
		{
			// Arrange
			var deviceId = "TestDevice";
			var device = new Device { Id = deviceId, Type = SensorType.Temperature };
			_mockRepository.Setup(r => r.GetDeviceAsync(deviceId))
				.ReturnsAsync(device);
			_mockRepository.Setup(r => r.GetDailyMaximumReadingsAsync(deviceId, 30))
				.ReturnsAsync(new List<DailyMaximumReading>());

			// Act
			var result = await _sensorService.GetDeviceStatisticsAsync(deviceId);

			// Assert
			Assert.Null(result);
		}
	}
}
