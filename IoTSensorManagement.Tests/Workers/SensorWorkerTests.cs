using IoTSensorManagement.Core.Interfaces;
using IoTSensorManagement.Core.Models;
using IoTSensorManagement.Shared.Configuration;
using IoTSensorManagement.Workers.Workers;
using Microsoft.Extensions.Logging;
using Moq;

namespace IoTSensorManagement.Tests.Workers
{
	public class SensorWorkerTests
	{
		private readonly Mock<ISensor> _mockSensor;
		private readonly Mock<IApiClient> _mockApiClient;
		private readonly Mock<ILogger<TemperatureSensorWorker>> _mockLogger;
		private readonly SensorConfiguration _testConfig;

		public SensorWorkerTests()
		{
			_mockSensor = new Mock<ISensor>();
			_mockApiClient = new Mock<IApiClient>();
			_mockLogger = new Mock<ILogger<TemperatureSensorWorker>>();
			_testConfig = new SensorConfiguration
			{
				DeviceId = "TestDevice",
				Type = SensorType.Temperature,
				ReadingInterval = TimeSpan.FromSeconds(1),
				ReportingInterval = TimeSpan.FromSeconds(5)
			};

			_mockSensor.Setup(s => s.DeviceId).Returns(_testConfig.DeviceId);
			_mockSensor.Setup(s => s.Type).Returns(_testConfig.Type);
			_mockSensor.Setup(s => s.ReadingInterval).Returns(_testConfig.ReadingInterval);
			_mockSensor.Setup(s => s.ReportingInterval).Returns(_testConfig.ReportingInterval);
		}

		[Fact]
		public async Task ExecuteAsync_GeneratesReadingsAtCorrectInterval()
		{
			// Arrange
			var worker = new TemperatureSensorWorker(_mockSensor.Object, _mockApiClient.Object, _mockLogger.Object, _testConfig);
			var cts = new CancellationTokenSource();

			_mockSensor.Setup(s => s.GenerateDataAsync())
				.ReturnsAsync(new TemperatureSensorData { Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Temperature = 25.0 });

			// Act
			var executionTask = worker.StartAsync(cts.Token);

			// Allow the worker to run for a short time
			await Task.Delay(TimeSpan.FromSeconds(3.5));
			await worker.StopAsync(cts.Token);

			// Assert
			_mockSensor.Verify(s => s.GenerateDataAsync(), Times.AtLeast(3));
			_mockSensor.Verify(s => s.GenerateDataAsync(), Times.AtMost(4));
		}

		[Fact]
		public async Task ExecuteAsync_SendsDataAtCorrectInterval()
		{
			// Arrange
			var worker = new TemperatureSensorWorker(_mockSensor.Object, _mockApiClient.Object, _mockLogger.Object, _testConfig);
			var cts = new CancellationTokenSource();

			var sensorData = new List<ISensorData>
			{
				new TemperatureSensorData { Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Temperature = 25.0 }
			};

			_mockSensor.Setup(s => s.GenerateDataAsync())
				.ReturnsAsync(sensorData[0]);
			_mockSensor.Setup(s => s.GetBufferedDataAsync())
				.ReturnsAsync(sensorData);

			// Act
			var executionTask = worker.StartAsync(cts.Token);

			// Allow the worker to run for a short time
			await Task.Delay(TimeSpan.FromSeconds(7));
			await worker.StopAsync(cts.Token);

			// Assert
			_mockApiClient.Verify(c => c.SendDataAsync(_testConfig.DeviceId, It.IsAny<IEnumerable<ISensorData>>()), Times.Once);
		}

		[Fact]
		public async Task ExecuteAsync_ClearsBufferAfterSending()
		{
			// Arrange
			var worker = new TemperatureSensorWorker(_mockSensor.Object, _mockApiClient.Object, _mockLogger.Object, _testConfig);
			var cts = new CancellationTokenSource();

			var sensorData = new List<ISensorData>
			{
				new TemperatureSensorData { Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Temperature = 25.0 }
			};

			_mockSensor.Setup(s => s.GenerateDataAsync())
				.ReturnsAsync(sensorData[0]);
			_mockSensor.Setup(s => s.GetBufferedDataAsync())
				.ReturnsAsync(sensorData);

			// Act
			var executionTask = worker.StartAsync(cts.Token);

			// Allow the worker to run for a short time
			await Task.Delay(TimeSpan.FromSeconds(7));
			await worker.StopAsync(cts.Token);

			// Assert
			_mockSensor.Verify(s => s.ClearBuffer(), Times.Once);
		}
	}
}
