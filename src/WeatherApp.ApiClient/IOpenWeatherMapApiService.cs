﻿using Refit;
using System.Threading.Tasks;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.Interfaces
{
    public interface IOpenWeatherAppApiService
    {
        [Get("/onecall?lat={lat}&lon={lon}&exclude=hourly,minutely,alterts&appid={apiKey}&units=imperial")]
        public Task<ApiResponse<WeatherData>> GetWeatherInfo(string lat, string lon, string apiKey);
    }
}