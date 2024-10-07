# IoT Sensor Management System

## Table of Contents
1. [Introduction](#introduction)
2. [System Architecture](#system-architecture)
3. [Key Components](#key-components)
4. [Getting Started](#getting-started)
5. [Configuration](#configuration)
6. [Adding New Sensors](#adding-new-sensors)
7. [Adding Support for New Sensor Types](#adding-support-for-new-sensor-types)
8. [API Endpoints](#api-endpoints)
9. [Data Storage](#data-storage)
10. [Deployment](#deployment)

## Introduction

The IoT Sensor Management System is a comprehensive solution designed to collect, process, and manage data from various types of IoT sensors. This system supports multiple sensor types, including light, temperature, and humidity sensors, with the flexibility to add more sensor types as needed.

Key features of the system include:
- Real-time data collection from IoT sensors
- Configurable reading and reporting intervals for each sensor
- RESTful API for data retrieval and management
- Scalable architecture to support a growing number of sensors
- In-memory and database storage options
- Detailed logging and error handling

## System Architecture

The system consists of two main components:

1. **Sensor Data Collection Service**: Responsible for simulating sensors, collecting data, and sending it to the API.
2. **API Service**: Handles data ingestion, storage, and retrieval.

## Key Components

- **Core**: Contains interfaces, models, and core services used across the system.
- **API**: Implements the RESTful API for data ingestion and retrieval.
- **Sensors**: Implements various sensor types and a factory for creating them.
- **Workers**: Background services that simulate sensor behavior and send data to the API.
- **Shared**: Contains shared configuration and utilities used across the system.

## Getting Started

To set up and run the IoT Sensor Management System:

1. Clone the repository:
   ```
   git clone https://github.com/mertyagmur/IoTSensorManagement.git
   ```

2. Navigate to the project directory:
   ```
   cd IoTSensorManagement
   ```

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Update the configuration files (`appsettings.json`) in both the API and Worker projects with your specific settings.

5. Set Up Multiple Startup Projects:
   - In **Solution Explorer**, right-click on the solution name and select **Properties**.
   - In the **Solution Properties** window, navigate to the **Common Properties** section and click on **Startup Project**.
   - Select the **Multiple startup projects** option.
   - In the list of projects, set both:
     - **IoTSensorManagement.Api** to **Start**
     - **IoTSensorManagement.Workers** to **Start**

6. Run the Projects:
   - Press **F5** or click the **Start** button in the Visual Studio toolbar. This will launch both the API and Worker projects in separate console windows.

## Configuration

The system uses JSON configuration files for both the API and Worker services. Key configuration files include:

- `appsettings.json`: Contains general application settings, logging configuration, and database connection strings.
- `config.json`: Defines sensor configurations, including device IDs, types, and intervals.

Make sure to update these files with your specific settings before running the application.

## Adding New Sensors

New sensors can be added to the system through the `config.json` file located in the `IoTSensorManagement.Shared` namespace.

   - Example:
     ```json
     [
        {
          "DeviceId": "Light_001",
          "Type": "Light",
          "ReadingInterval": "00:15:00",
          "ReportingInterval": "01:00:10"
        },
        {
          "DeviceId": "Temp_001",
          "Type": "Temperature",
          "ReadingInterval": "00:00:02",
          "ReportingInterval": "00:00:10"
        },
        {
          "DeviceId": "Hum_001",
          "Type": "Humidity",
          "ReadingInterval": "00:00:30",
          "ReportingInterval": "00:01:00"
        }
      ]
     ```

## Adding Support for New Sensor Types

The system is designed to be easily extensible for adding new sensor types. To add a new sensor:

1. **Create a new sensor data model**:
   - Add a new class in the `IoTSensorManagement.Core.Models` namespace, implementing the `ISensorData` interface.
   - Example:
     ```csharp
     public class PressureSensorData : ISensorData
     {
         [JsonPropertyName("pressure")]
         public double Pressure { get; set; }

         [JsonPropertyName("time")]
         public long Timestamp { get; set; }

         [JsonIgnore]
         public SensorType SensorType => SensorType.Pressure;
     }
     ```

2. **Update the `SensorType` enum**:
   - Add the new sensor type to the `SensorType` enum in `IoTSensorManagement.Core.Models.SensorType`.

3. **Create a new sensor class**:
   - Add a new class in the `IoTSensorManagement.Sensors.Sensors` namespace, inheriting from `BaseSensor`.
   - Implement the `GenerateDataAsync` method to simulate data for the new sensor type.
   - Example:
     ```csharp
     public class PressureSensor : BaseSensor
     {
         public override SensorType Type => SensorType.Pressure;

         public PressureSensor(string deviceId, TimeSpan readingInterval, TimeSpan reportingInterval)
             : base(deviceId, readingInterval, reportingInterval) { }

         public override async Task<ISensorData> GenerateDataAsync()
         {
             // Implement pressure data generation logic
         }
     }
     ```

4. **Update the `SensorFactory`**:
   - Modify the `CreateSensor` method in `IoTSensorManagement.Sensors.Factories.SensorFactory` to include the new sensor type.

5. **Create a new worker class**:
   - Add a new class in the `IoTSensorManagement.Workers.Workers` namespace, inheriting from `SensorWorker`.
   - Example:
     ```csharp
     public class PressureSensorWorker : SensorWorker
     {
         public PressureSensorWorker(ISensor sensor, IApiClient apiClient, ILogger logger, SensorConfiguration config)
             : base(sensor, apiClient, logger, config) { }
     }
     ```

6. **Update the `WorkerFactory`**:
   - Modify the `CreateWorker` method in `IoTSensorManagement.Workers.Factories.WorkerFactory` to include the new worker type.
  
7. **Update the `SensorDataDto`**:
   - Modify the `SensorDataDto` in `IoTSensorManagement.Core.DTOs.SensorDataDto` to include the sensor data type.

8. **Update the `AutoMapperProfile`**:
   - Add mapping configurations and methods for the new sensor type in `IoTSensorManagement.Core.Mappings.AutoMapperProfile`.

9. **Update the `SensorService`**:
   - Modify the `GetAllDataAsync`, `GetDeviceStatisticsAsync`, `MapToSensorData`, and `GetTypedMaxValue` methods in `IoTSensorManagement.Core.Services.SensorService` to handle the new sensor type.
  
10. **Update the `ApiClient`**:
   - Modify the `ConvertToSensorDataDtos` method in `IoTSensorManagement.Core.Services.ApiClient` to handle the new sensor type.

11. **Update the repositories**:
   - Modify the repository implementations to handle the new sensor type.

12. **Update the configuration**:
   - Add a new entry for the new sensor type in the `config.json` file.

After completing these steps, the system will be able to handle the new sensor type, collect its data, and make it available through the API.

## API Endpoints

The system exposes the following RESTful API endpoints:

- `POST /devices/{deviceId}/telemetry`: Submit telemetry data for a specific device
- `GET /devices/{deviceId}`: Retrieve all data for a specific device
- `GET /devices/{deviceId}/statistics`: Get statistical data for a specific device

For detailed API documentation, refer to the Swagger UI available at `/swagger` when running the API project.

## Data Storage

The system supports two storage options:

1. **In-Memory Storage**: Useful for development and testing purposes.
2. **SQL Server Database**: For production use, providing persistent storage.

The storage option can be configured in the `appsettings.json` file of the API project.

## Deployment

For production deployment:

1. Build the projects:
   ```
   dotnet publish -c Release
   ```

2. Deploy the built files to your hosting environment (e.g., Azure App Service, Docker containers).

3. Ensure all configuration files are properly set up in the production environment.

4. Set up a SQL Server database if using database storage.

5. Configure proper authentication and security measures for the API.
