using Refit;
using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.Interfaces
{
    public interface IOpenWeatherMapApiService
    {
        [Get("/onecall?lat={lat}&lon={lon}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherData>> GetWeatherInfo(string lat, string lon, string apiKey);
    }
}