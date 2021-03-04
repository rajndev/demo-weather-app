using AutoMapper;
using WeatherApp.BLL.Models;
using WeatherApp.Web.ViewModels;

namespace WeatherApp.HelperClasses
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            //Home controller

            CreateMap<WeatherInfoDTO, CurrentWeatherViewModel>();

            //Weather Service

            CreateMap<WeatherInfoRoot, WeatherInfoDTO>()
                .ForMember(destProp => destProp.WeatherCondition, act => act.MapFrom(srcProp => srcProp.Weather[0].Description))
                .ForMember(destProp => destProp.Icon, act => act.MapFrom(srcProp => srcProp.Weather[0].Icon))
                .ForMember(destProp => destProp.Temperature, act => act.MapFrom(srcProp => srcProp.Main.Temp));
        }
    }
}