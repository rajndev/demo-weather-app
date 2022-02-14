using Refit;
using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.Interfaces
{
    public interface IOpenWeatherAppApiService
    {
        [Get("/weather?id={cityCode}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherInfoRoot>> GetWeatherInfoByCityCode(int? cityCode, string apiKey);

        [Get("/weather?q={cityName}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherInfoRoot>> GetWeatherInfoByCityName(string cityName, string apiKey);
    }
}