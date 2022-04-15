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
            CreateMap<ProviderResult<WeatherData>, ApiResponse<WeatherData>>().ReverseMap();
            CreateMap<CurrentWeatherViewModel, ProviderResult<WeatherData>>().ReverseMap()
                 .ForMember(destProp => destProp.WeatherCondition, act => act.MapFrom(srcProp => srcProp.Content.Current.Weather[0].Description))
                 .ForMember(destProp => destProp.Icon, act => act.MapFrom(srcProp => srcProp.Content.Current.Weather[0].Icon))
                 .ForMember(destProp => destProp.Temperature, act => act.MapFrom(srcProp => srcProp.Content.Current.Temp))
                 .ForMember(destProp => destProp.CityName, act => act.MapFrom(srcProp => srcProp.Content.CityName))
                 .ForMember(destProp => destProp.CityDate, act => act.MapFrom(srcProp => srcProp.Content.CityDate))
                 .ForMember(destProp => destProp.CityTime, act => act.MapFrom(srcProp => srcProp.Content.CityTime))
                 .ForMember(destProp => destProp.IsDayTime, act => act.MapFrom(srcProp => srcProp.Content.IsDayTime))
                 .ForMember(destProp => destProp.DailyForecast, act => act.MapFrom(srcProp => srcProp.Content.Daily));
        }
    }
}