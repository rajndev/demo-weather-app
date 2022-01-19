using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherInfoDTO> GetCurrentWeather(string cityName, string apiKey);
    }
}
