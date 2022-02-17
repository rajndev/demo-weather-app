using AutoMapper;
using GoogleMaps.LocationServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.Common.Models;
using WeatherApp.Data.Provider.DataContext;
using WeatherApp.Data.Provider.Entities;
using WeatherApp.Provider.Interfaces;

namespace WeatherApp.Provider
{
    public class WeatherServiceProvider : IWeatherService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IOpenWeatherAppApiService _apiService;
        private readonly ApiOptions _options;

        public WeatherServiceProvider(
                IMapper mapper,
                ApplicationDbContext context,
                IConfiguration config,
                IOpenWeatherAppApiService apiService,
                IOptions<ApiOptions> options
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

        public async Task<ProviderResult<WeatherData>> GetCurrentWeather(string cityName)
        {
            var fdsa = GetCityCoordinates(cityName);

            var split = cityName.Split(",");

            int? cityCode = await GetCityCodeAsync(split, split.Length);

            var apiResponse = cityCode != null ?
                await _apiService.GetWeatherInfoByCityCode(cityCode, _options.OpenWeatherMapApiKey) :
                await _apiService.GetWeatherInfoByCityName(cityName, _options.OpenWeatherMapApiKey);

            var providerResult = _mapper.Map<ProviderResult<WeatherData>>(apiResponse);

            if ((int)apiResponse.StatusCode == 200)
            {
                var currentDateTime = GetDateTimeFromEpoch(
                        apiResponse.Content.Sys.Sunrise,
                        apiResponse.Content.Sys.Sunset,
                        apiResponse.Content.Dt,
                        apiResponse.Content.Timezone
                    );

                providerResult.Content.CityDate = currentDateTime.Item1;
                providerResult.Content.CityTime = currentDateTime.Item2;
                providerResult.Content.IsDayTime = currentDateTime.Item3;

                //capitalize each word in the city name
                cityName = CapitalizeCityName(cityName);
                providerResult.Content.CityName = cityName;
            }

            return providerResult;
        }

        public Tuple<string, string> GetCityCoordinates(string cityName)
        {
            // var address = cityName;

            var locationService = new GoogleLocationService(_options.GoogleApiKey);
            var point = locationService.GetLatLongFromAddress("Pittsburgh, PA");

            var latitude = point.Latitude.ToString();
            var longitude = point.Longitude.ToString();

            return Tuple.Create(latitude, longitude);
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

            var displayDate = dateTimeOffset.DateTime.ToString("D");
            var displayTime = dateTimeOffset.DateTime.ToString("t");

            return Tuple.Create(displayDate, displayTime, isDaytime);
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