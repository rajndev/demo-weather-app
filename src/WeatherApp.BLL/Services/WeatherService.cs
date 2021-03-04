using System;
using System.Globalization;
using System.Threading.Tasks;
using WeatherApp.BLL.HelperClasses;
using WeatherApp.BLL.Interfaces;

namespace WeatherApp.BLL.Services
{
    public class WeatherService : IWeatherService
    {
        public WeatherAPIProcessor APIProcessorSingleton = WeatherAPIProcessor.GetInstance();
        public async Task<object> GetCurrentWeather(string cityName, string apiKey)
        {
            APIProcessorSingleton.BaseAPIUrl = BaseAPIUrls.GET_CURRENT_WEATHER;
            var query = $"q={cityName}&appid={apiKey}&units=imperial";
            var currentWeather = await APIProcessorSingleton.GetCurrentWeather(query);

            if (currentWeather != null)
            {
                if (currentWeather.GetType() == typeof(string))
                {
                    return "invalid city name";
                }
                else
                {
                    var currentWeatherCast = (WeatherInfoRoot)currentWeather;
                    var currentDateTime = GetDateTimeFromEpoch(currentWeatherCast.Sys.Sunrise, currentWeatherCast.Sys.Sunset, currentWeatherCast.Dt);

                    currentWeatherCast.CurrentDate = currentDateTime.Item1;
                    currentWeatherCast.CurrentTime = currentDateTime.Item2;
                    currentWeatherCast.isDayTime = currentDateTime.Item3;

                    //capitalize each word in the city name

                    cityName = CapitalizeText(cityName);

                    currentWeatherCast.CityName = cityName;

                    return currentWeatherCast;
                }
            }

            return "service unavailable";
        }

        private string CapitalizeText(string cityName)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            cityName = textInfo.ToTitleCase(cityName);
            return cityName;
        }

        private Tuple<string, string, bool> GetDateTimeFromEpoch(int sunrise, int sunset, int currentTime)
        {
            DateTime dtime = new DateTime(1970, 1, 1, 0, 0, 0);

            var dtimeCurrent = dtime.AddSeconds(currentTime);

            bool isDaytime = currentTime > sunrise && currentTime < sunset;

            var humanReadabledate = dtimeCurrent.ToString("D");
            var HumanReadabletime = dtimeCurrent.ToString("t");

            return Tuple.Create(humanReadabledate, HumanReadabletime, isDaytime);
        }
    }
}
