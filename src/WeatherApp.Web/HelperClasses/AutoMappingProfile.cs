using AutoMapper;
using WeatherApp.Common.Models;
using WeatherApp.Web.ViewModels;
using Refit;

namespace WeatherApp.HelperClasses
{
    public class AutoMappingProfile : Profile
    {
        public AutoMappingProfile()
        {
            //Home controller

            //CreateMap<WeatherInfoDto, CurrentWeatherViewModel>().ReverseMap();

            //Weather Service
            /*
                        CreateMap<WeatherInfoRoot, WeatherInfoDto>().ReverseMap()
                            .ForMember(destProp => destProp.WeatherCondition, act => act.MapFrom(srcProp => srcProp.Weather[0].Description))
                            .ForMember(destProp => destProp.Icon, act => act.MapFrom(srcProp => srcProp.Weather[0].Icon))
                            .ForMember(destProp => destProp.Temperature, act => act.MapFrom(srcProp => srcProp.Main.Temp));*/

            CreateMap<ApiResponse<WeatherInfoRoot>, ApiResult<WeatherInfoRoot>>().ReverseMap();
            CreateMap<CurrentWeatherViewModel, ApiResult<WeatherInfoRoot>>().ReverseMap()
                 .ForMember(destProp => destProp.WeatherCondition, act => act.MapFrom(srcProp => srcProp.Content.Weather[0].Description))
                 .ForMember(destProp => destProp.Icon, act => act.MapFrom(srcProp => srcProp.Content.Weather[0].Icon))
                 .ForMember(destProp => destProp.Temperature, act => act.MapFrom(srcProp => srcProp.Content.Main.Temp));
        }
    }
}