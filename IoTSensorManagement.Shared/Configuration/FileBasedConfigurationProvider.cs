using IoTSensorManagement.Shared.Converters;
using IoTSensorManagement.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace IoTSensorManagement.Shared.Configuration
{
    public class FileBasedConfigurationProvider : ISharedConfigurationProvider
    {
        private readonly string _configPath;
        private readonly ILogger<FileBasedConfigurationProvider> _logger;

        public FileBasedConfigurationProvider(string configPath, ILogger<FileBasedConfigurationProvider> logger)
        {
            _configPath = configPath ?? throw new ArgumentNullException(nameof(configPath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IEnumerable<SensorConfiguration> GetSensorConfigurations()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _logger.LogError("Configuration file not found: {ConfigPath}", _configPath);
                    return Array.Empty<SensorConfiguration>();
                }

                string jsonContent = File.ReadAllText(_configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new SensorTypeJsonConverter() }
                };

                var sensorConfigurations = JsonSerializer.Deserialize<List<SensorConfiguration>>(jsonContent, options);

                if (sensorConfigurations == null || sensorConfigurations.Count == 0)
                {
                    _logger.LogWarning("No sensor configurations found in the file: {ConfigPath}", _configPath);
                    return Array.Empty<SensorConfiguration>();
                }

                _logger.LogInformation("Successfully loaded {Count} sensor configurations from {ConfigPath}",
                    sensorConfigurations.Count, _configPath);

                return sensorConfigurations;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing JSON in configuration file: {ConfigPath}", _configPath);
                return Array.Empty<SensorConfiguration>();
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading configuration file: {ConfigPath}", _configPath);
                return Array.Empty<SensorConfiguration>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while reading configuration file: {ConfigPath}", _configPath);
                return Array.Empty<SensorConfiguration>();
            }
        }
    }
}
