using AutoMapper;
using GoogleMaps.LocationServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NodaTime;
using System.Globalization;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.Common.Models;
using WeatherApp.Data.Provider.DataContext;
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
            var cityCoordinates = GetCityCoordinates(cityName);
            var lat = cityCoordinates?.Item1;
            var lon = cityCoordinates?.Item2;

            var apiResponse = await _apiService.GetWeatherInfo(lat, lon, _options.OpenWeatherMapApiKey);

            var providerResult = _mapper.Map<ProviderResult<WeatherData>>(apiResponse);

            if ((int)apiResponse.StatusCode == 200)
            {
                var zoneDateTime = GetZoneDateTime(
                    apiResponse.Content.TimeZone,
                    apiResponse.Content.Current.Dt,
                    apiResponse.Content.Current.Sunrise,
                    apiResponse.Content.Current.Sunset);

                providerResult.Content.CityDate = zoneDateTime.Item1;
                providerResult.Content.CityTime = zoneDateTime.Item2;
                providerResult.Content.IsDayTime = zoneDateTime.Item3;
                
                cityName = CapitalizeCityName(cityName);
                providerResult.Content.CityName = cityName;
            }

            return providerResult;
        }

        public Tuple<string, string> GetCityCoordinates(string cityName)
        {
            var locationService = new GoogleLocationService(_options.GoogleApiKey);
            var point = locationService.GetLatLongFromAddress(cityName);

            if (point != null)
            {
                var lat = point.Latitude.ToString();
                var lon = point.Longitude.ToString();
                return Tuple.Create(lat, lon);
            }
            return null;
        }

        private string CapitalizeCityName(string cityName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            cityName = textInfo.ToTitleCase(cityName);
            return cityName;
        }

        private Tuple<string, string, bool> GetZoneDateTime(string timeZone, long currentTime, long sunrise, long sunset)
        {
            var now = SystemClock.Instance.GetCurrentInstant();

            var dtzi = DateTimeZoneProviders.Tzdb;
            var timeZoneToken = dtzi[timeZone];

            var seattleNow = new ZonedDateTime(now, timeZoneToken);

            var displayDate = seattleNow.LocalDateTime.ToDateTimeUnspecified().ToString("D");
            var displayTime = seattleNow.LocalDateTime.ToDateTimeUnspecified().ToShortTimeString();

            bool isDaytime = currentTime > sunrise && currentTime < sunset;

            return Tuple.Create(displayDate, displayTime, isDaytime);
        }
    }
}