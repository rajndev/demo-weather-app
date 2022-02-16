using Refit;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.Interfaces
{
    public interface IOpenWeatherAppApiService
    {
        [Get("/weather?id={cityCode}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherData>> GetWeatherInfoByCityCode(int? cityCode, string apiKey);

        [Get("/weather?q={cityName}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherData>> GetWeatherInfoByCityName(string cityName, string apiKey);
    }
}