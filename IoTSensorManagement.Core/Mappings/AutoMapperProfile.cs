using AutoMapper;
using IoTSensorManagement.Api.DTOs;
using IoTSensorManagement.Core.Models;

namespace IoTSensorManagement.Core.Mappings
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			// SensorDataDto to SensorData mappings
			CreateMap<SensorDataDto, SensorData>()
				.ForMember(dest => dest.SensorType, opt => opt.MapFrom(src => GetSensorType(src)))
				.ForMember(dest => dest.Value, opt => opt.MapFrom(src => GetSensorValue(src)));

			// SensorData to SensorDataDto mappings
			CreateMap<SensorData, SensorDataDto>()
				.ForMember(dest => dest.Illuminance, opt => opt.MapFrom(src => src.SensorType == SensorType.Light ? src.Value : (double?)null))
				.ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.SensorType == SensorType.Temperature ? src.Value : (double?)null))
				.ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.SensorType == SensorType.Humidity ? src.Value : (double?)null));

			// Direct mappings for specific sensor data types
			CreateMap<SensorDataDto, LightSensorData>()
				.ForMember(dest => dest.Illuminance, opt => opt.MapFrom(src => src.Illuminance ?? 0))
				.ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

			CreateMap<SensorDataDto, TemperatureSensorData>()
				.ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature ?? 0))
				.ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

			CreateMap<SensorDataDto, HumiditySensorData>()
				.ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Humidity ?? 0))
				.ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp));

			// Reverse mappings for specific sensor data types
			CreateMap<LightSensorData, SensorDataDto>()
				.ForMember(dest => dest.Illuminance, opt => opt.MapFrom(src => src.Illuminance));

			CreateMap<TemperatureSensorData, SensorDataDto>()
				.ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature));

			CreateMap<HumiditySensorData, SensorDataDto>()
				.ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.Humidity));
		}

		private SensorType GetSensorType(SensorDataDto dto)
		{
			if (dto.Illuminance.HasValue) return SensorType.Light;
			if (dto.Temperature.HasValue) return SensorType.Temperature;
			if (dto.Humidity.HasValue) return SensorType.Humidity;
			throw new ArgumentException("Unable to determine sensor type from DTO");
		}

		private double? GetSensorValue(SensorDataDto dto)
		{
			return dto.Illuminance ?? dto.Temperature ?? dto.Humidity;
		}
	}
}
