using System;
using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.Provider.Interfaces
{
    public interface IWeatherService
    {
        Task<ProviderResult<WeatherData>> GetCurrentWeather(string cityName);

        Tuple<string, string, string, string, string, bool> GetZoneDateTime(string timeZone, long currentTime = 0, long sunrise = 0, long sunset = 0);
    }
}