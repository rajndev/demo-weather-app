using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Models;
using WeatherApp.DAL.Data;
using WeatherApp.DAL.Entities;

namespace WeatherApp.BLL.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly IOpenWeatherAppApiService _apiService;

        public WeatherService(IMapper mapper, ApplicationDbContext context, IConfiguration config)
        {
            _mapper = mapper;
            _context = context;
            _config = config;
            _apiKey = _config.GetValue<string>("OpenWeatherMap:APIKey");
            _apiService = RestService.For<IOpenWeatherAppApiService>("http://api.openweathermap.org/data/2.5");
        }

        public async Task<WeatherInfoDto> GetCurrentWeather(string cityName)
        {
            int? cityCode = null;
            WeatherInfoDto weatherInfoDTO = new WeatherInfoDto();
            ApiResponse<WeatherInfoRoot> apiResponse;

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
                apiResponse = await _apiService.GetWeatherInfoByCityCode(cityCode, _apiKey);
            }
            else
            {
                apiResponse = await _apiService.GetWeatherInfoByCityName(cityName, _apiKey);
            }

            if ((int)apiResponse.StatusCode == 200)
            {
                weatherInfoDTO = _mapper.Map<WeatherInfoDto>(apiResponse.Content);

                var currentDateTime = GetDateTimeFromEpoch(
                    apiResponse.Content.Sys.Sunrise,
                    apiResponse.Content.Sys.Sunset,
                    apiResponse.Content.Dt,
                    apiResponse.Content.Timezone
                    );

                weatherInfoDTO.CityDate = currentDateTime.Item1;
                weatherInfoDTO.CityTime = currentDateTime.Item2;
                weatherInfoDTO.IsDayTime = currentDateTime.Item3;
                weatherInfoDTO.isStatusOK = true;

                //capitalize each word in the city name
                cityName = CapitalizeCityName(cityName);
                weatherInfoDTO.CityName = cityName;
            }
            else if ((int)apiResponse.StatusCode == 404)
            {
                weatherInfoDTO.isStatusNotFound = true;
            }
            else
            {
                weatherInfoDTO.isStatusOther = true;
            }

            return weatherInfoDTO;
        }

        private string CapitalizeCityName(string cityName)
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