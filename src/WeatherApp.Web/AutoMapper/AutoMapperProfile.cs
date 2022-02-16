using AutoMapper;
using WeatherApp.Common.Models;
using WeatherApp.Web.ViewModels;
using Refit;

namespace WeatherApp.Web.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<WeatherResult<WeatherData>, ApiResponse<WeatherData>>().ReverseMap();
            CreateMap<CurrentWeatherViewModel, WeatherResult<WeatherData>>().ReverseMap()
                 .ForMember(destProp => destProp.WeatherCondition, act => act.MapFrom(srcProp => srcProp.Content.Weather[0].Description))
                 .ForMember(destProp => destProp.Icon, act => act.MapFrom(srcProp => srcProp.Content.Weather[0].Icon))
                 .ForMember(destProp => destProp.Temperature, act => act.MapFrom(srcProp => srcProp.Content.Main.Temp));
        }
    }
}