using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.Provider.Interfaces
{
    public interface IWeatherService
    {
        Task<ProviderResult<WeatherData>> GetCurrentWeather(string cityName);
    }
}