using AutoMapper;
using System;
using System.Globalization;
using System.Threading.Tasks;
using WeatherApp.BLL.HelperClasses;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMapper _mapper;
        private WeatherAPIProcessor _apiProcessorSingleton;
        private WeatherInfoRoot _apiResponse;
        public WeatherService(IMapper mapper)
        {
            _mapper = mapper;
            _apiProcessorSingleton = WeatherAPIProcessor.GetInstance();
            _apiProcessorSingleton.BaseAPIUrl = BaseAPIUrls.GET_CURRENT_WEATHER;
        }

        public async Task<WeatherInfoDTO> GetCurrentWeather(string apiKey, string cityName = null, int? cityId = null)
        {
            int httpResponse;
            WeatherInfoDTO weatherInfoDTO = new WeatherInfoDTO();

            if (cityId != null)
            {
                var query = $"id={cityId}&appid={apiKey}&units=imperial";
                httpResponse = await _apiProcessorSingleton.CallWeatherApi(query);
            }
            else
            {
                var query = $"q={cityName}&appid={apiKey}&units=imperial";
                httpResponse = await _apiProcessorSingleton.CallWeatherApi(query);
            }

            if(httpResponse == 200)
            {
                _apiResponse = _apiProcessorSingleton.GetApiResponseData();

                weatherInfoDTO = _mapper.Map<WeatherInfoDTO>(_apiResponse);

                var currentDateTime = GetDateTimeFromEpoch(
                    _apiResponse.Sys.Sunrise,
                     _apiResponse.Sys.Sunset,
                     _apiResponse.Dt,
                     _apiResponse.Timezone);

                weatherInfoDTO.CityDate = currentDateTime.Item1;
                weatherInfoDTO.CityTime = currentDateTime.Item2;
                weatherInfoDTO.IsDayTime = currentDateTime.Item3;
                weatherInfoDTO.isStatusOK = true;

                //capitalize each word in the city name

                cityName = CapitalizeText(cityName);
                weatherInfoDTO.CityName = cityName;
            }
            else if(httpResponse == 404)
            {
                weatherInfoDTO.isStatusNotFound = true;
            }
            else
            {
                weatherInfoDTO.isStatusOther = true;
            }

            return weatherInfoDTO;
        }

        private string CapitalizeText(string cityName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            cityName = textInfo.ToTitleCase(cityName);
            return cityName;
        }

        private Tuple<string, string, bool> GetDateTimeFromEpoch(long sunrise, long sunset, long currentTime, long timezone)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(currentTime + timezone);

            bool isDaytime = currentTime > sunrise && currentTime < sunset;

            var humanReadableDate = dateTimeOffset.DateTime.ToString("D");
            var humanReadableTime = dateTimeOffset.DateTime.ToString("t");

            return Tuple.Create(humanReadableDate, humanReadableTime, isDaytime);
        }
    }
}
