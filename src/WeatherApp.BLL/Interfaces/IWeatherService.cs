using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherService
    {
        Task<ApiResult<WeatherInfoRoot>> GetCurrentWeather(string cityName);
    }
}