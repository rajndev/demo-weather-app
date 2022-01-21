using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.BLL.HelperClasses;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Models;
using WeatherApp.DAL.Data;
using WeatherApp.DAL.Entities;

namespace WeatherApp.BLL.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMapper _mapper;
        private WeatherApiProcessor _apiProcessorSingleton;
        private WeatherInfoRoot _apiResponse;
        private readonly ApplicationDbContext _context;
        public WeatherService(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _apiProcessorSingleton = WeatherApiProcessor.GetInstance();
            _apiProcessorSingleton.BaseAPIUrl = BaseApiUrls.GET_CURRENT_WEATHER;
            _context = context;
        }

        public async Task<WeatherInfoDto> GetCurrentWeather(string cityName, string apiKey)
        {
            int httpResponse;
            int? cityCode = null;
            WeatherInfoDto weatherInfoDTO = new WeatherInfoDto();

            var split = cityName.Split(",");

            if (split.Length == 3)
            {
                cityCode = await GetCityCode(split, 3);
            }
            else if (split.Length == 2)
            {
                cityCode = await GetCityCode(split, 2);
            }

            if (cityCode != null)
            {
                var query = $"id={cityCode}&appid={apiKey}&units=imperial";
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

                weatherInfoDTO = _mapper.Map<WeatherInfoDto>(_apiResponse);

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

        public async Task<int?> GetCityCode(string[] split, int length)
        {
            City cityRecord = null;

            if (length == 3)
            {
                cityRecord = await _context.Cities.Where(p => p.Name == split[0].Trim() && p.State == split[1].Trim() && p.Country == split[2].Trim()).FirstOrDefaultAsync();
            }
            else if (length == 2)
            {
                cityRecord = await _context.Cities.Where(p => p.Name == split[0].Trim() && p.State == split[1].Trim()).FirstOrDefaultAsync();
            }

            return cityRecord?.CityCode;
        }
    }
}
