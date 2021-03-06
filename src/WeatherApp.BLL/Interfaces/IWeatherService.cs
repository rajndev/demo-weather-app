using System.Threading.Tasks;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherService
    {
        Task<object> GetCurrentWeather(string apiKey, string cityName = null, int cityId = 0);
    }
}
