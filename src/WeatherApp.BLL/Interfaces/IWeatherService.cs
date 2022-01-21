using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherInfoDto> GetCurrentWeather(string cityName, string apiKey);
    }
}
