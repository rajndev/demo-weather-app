using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.BLL.Models;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using WeatherApp.BLL.Interfaces;

namespace WeatherApp.BLL.HelperClasses
{
    public class WeatherApiProcessor : IWeatherApiProcessor
    {
        private static HttpClient _apiClient = new HttpClient();
        private WeatherInfoRoot _apiResponseData;
        private readonly IConfiguration _config;
        private string _apiKey;
        public const string GET_CURRENT_WEATHER = "http://api.openweathermap.org/data/2.5/weather?";

        public string ApiKey
        {
            get { return _apiKey; }
        }

        public WeatherApiProcessor(IConfiguration config)
        {
            _config = config;

            SetApiKey();
        }

        private void SetApiKey()
        {
            _apiKey = _config.GetValue<string>("OpenWeatherMap:APIKey");
        }

        public async Task<int> CallWeatherApi(string query)
        {
            var response = await _apiClient.GetAsync($"{GET_CURRENT_WEATHER}{query}");

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