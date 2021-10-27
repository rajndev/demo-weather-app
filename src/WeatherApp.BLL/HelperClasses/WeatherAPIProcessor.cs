﻿using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.HelperClasses
{ //Todo: Json parser from API instead of automapper
    public class WeatherAPIProcessor
    {
        private WeatherAPIProcessor() { }

        private static WeatherAPIProcessor _instance;
        private static HttpClient _apiClient = new HttpClient();
        private WeatherInfoRoot _apiResponseData;
        public string BaseAPIUrl { get; set; }

        public static WeatherAPIProcessor GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WeatherAPIProcessor();
            }
            return _instance;
        }

        public async Task<int> CallWeatherApi(string query)
        {
            var response = await _apiClient.GetAsync($"{BaseAPIUrl}{query}");

            if (response.IsSuccessStatusCode)
            {
                _apiResponseData = JsonConvert.DeserializeObject<WeatherInfoRoot>(await response.Content.ReadAsStringAsync());
            }

            var statusCode = (int)response.StatusCode;

            return statusCode;

        }

        public WeatherInfoRoot GetApiResponseData()
        {
            return _apiResponseData;
        }
    }
}
