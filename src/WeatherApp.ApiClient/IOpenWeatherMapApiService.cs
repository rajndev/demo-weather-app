using Refit;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.Interfaces
{
    public interface IOpenWeatherAppApiService
    {
        [Get("/onecall?lat={lat}&lon={lon}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherData>> GetWeatherInfo(string lat, string lon, string apiKey);
    }
}