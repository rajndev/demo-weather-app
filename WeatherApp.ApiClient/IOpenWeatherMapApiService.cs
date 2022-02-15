using Refit;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.Interfaces
{
    public interface IOpenWeatherAppApiService
    {
        [Get("/weather?id={cityCode}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherInfoRoot>> GetWeatherInfoByCityCode(int? cityCode, string apiKey);

        [Get("/weather?q={cityName}&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherInfoRoot>> GetWeatherInfoByCityName(string cityName, string apiKey);
    }
}