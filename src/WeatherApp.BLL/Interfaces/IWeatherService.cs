using System.Threading.Tasks;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherService
    {
        Task<object> GetCurrentWeather(string cityName, string apiKey);
    }
}
