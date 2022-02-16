using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.Provider.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherResult<WeatherData>> GetCurrentWeather(string cityName);
    }
}