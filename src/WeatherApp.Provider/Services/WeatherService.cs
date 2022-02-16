using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.Common.Models;
using WeatherApp.Data.Provider.DataContext;
using WeatherApp.Data.Provider.Entities;
using WeatherApp.Provider.Interfaces;

namespace WeatherApp.Provider.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IOpenWeatherAppApiService _apiService;
        private readonly OpenWeatherMapApiOptions _options;

        public WeatherService(
                IMapper mapper, 
                ApplicationDbContext context, 
                IConfiguration config, 
                IOpenWeatherAppApiService apiService, 
                IOptions<OpenWeatherMapApiOptions> options
            )
        {
            _ = options ?? throw new NullReferenceException(nameof(options));
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            _options = options.Value;
            _context = context;
            _apiService = apiService;
            _mapper = mapper;
        }

        public async Task<WeatherResult<WeatherData>> GetCurrentWeather(string cityName)
        {
            int? cityCode = null;

            var split = cityName.Split(",");

            cityCode = await GetCityCodeAsync(split, split.Length);

            var apiResponse = cityCode != null ? 
                await _apiService.GetWeatherInfoByCityCode(cityCode, _options.ApiKey) :
                await _apiService.GetWeatherInfoByCityName(cityName, _options.ApiKey);

            var weatherResult = _mapper.Map<WeatherResult<WeatherData>>(apiResponse);

            if ((int)apiResponse.StatusCode == 200)
            {
                var currentDateTime = GetDateTimeFromEpoch(
                        apiResponse.Content.Sys.Sunrise,
                        apiResponse.Content.Sys.Sunset,
                        apiResponse.Content.Dt,
                        apiResponse.Content.Timezone
                    );

              /*  weatherResult.CityDate = currentDateTime.Item1;
                weatherResult.CityTime = currentDateTime.Item2;
                weatherResult.IsDayTime = currentDateTime.Item3;*/

                //capitalize each word in the city name
               /* cityName = CapitalizeCityName(cityName);
                weatherResult.CityName = cityName;*/
            }

            return weatherResult;
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