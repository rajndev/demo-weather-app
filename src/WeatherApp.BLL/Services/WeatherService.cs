using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refit;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.BLL.Interfaces;
using WeatherApp.Common.Models;
using WeatherApp.DAL.Data;
using WeatherApp.DAL.Entities;

namespace WeatherApp.BLL.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IOpenWeatherAppApiService _apiService;
        private readonly string _apiKey;

        public WeatherService(IMapper mapper, ApplicationDbContext context, IConfiguration config, IOpenWeatherAppApiService apiService)
        {
            _mapper = mapper;
            _context = context;
            _apiService = apiService;
            _apiKey = config.GetValue<string>("OpenWeatherMapApiOptions:APIKey");
        }

        public async Task<ApiResult<WeatherInfoRoot>> GetCurrentWeather(string cityName)
        {
            int? cityCode = null;
            var apiResponseDto = new ApiResult<WeatherInfoRoot>();

            var split = cityName.Split(",");

            cityCode = await GetCityCodeAsync(split, split.Length);

            var apiResponse = cityCode != null ? await _apiService.GetWeatherInfoByCityCode(cityCode, _apiKey) : await _apiService.GetWeatherInfoByCityName(cityName, _apiKey);


            apiResponseDto = _mapper.Map<ApiResult<WeatherInfoRoot>>(apiResponse);


            if ((int)apiResponse.StatusCode == 200)
            {                /*                var apiResult = new ApiResult<WeatherInfoDto>()
                                {
                                    StatusCode = api
                                }*/

                var currentDateTime = GetDateTimeFromEpoch(
                        apiResponse.Content.Sys.Sunrise,
                        apiResponse.Content.Sys.Sunset,
                        apiResponse.Content.Dt,
                        apiResponse.Content.Timezone
                    );

                apiResponseDto.CityDate = currentDateTime.Item1;
                apiResponseDto.CityTime = currentDateTime.Item2;
                apiResponseDto.IsDayTime = currentDateTime.Item3;

                //capitalize each word in the city name
                cityName = CapitalizeCityName(cityName);
                apiResponseDto.CityName = cityName;
            }

            return apiResponseDto;
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

        public async Task<int?> GetCityCodeAsync(string[] split, int length)
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