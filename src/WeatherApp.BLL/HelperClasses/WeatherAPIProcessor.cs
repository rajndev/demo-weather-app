using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.HelperClasses
{ //Todo: Json parser from API instead of automapper
    public class WeatherAPIProcessor
    {
        private WeatherAPIProcessor() { }

        private static WeatherAPIProcessor _instance;
        public static HttpClient ApiClient;
        public string BaseAPIUrl { get; set; }

        public static WeatherAPIProcessor GetInstance()
        {
            if (_instance == null)
            {
                _instance = new WeatherAPIProcessor();
            }
            return _instance;
        }

        public async Task<object> GetCurrentWeather(string query)
        {
            ApiClient = new HttpClient();

            var response = await ApiClient.GetAsync($"{BaseAPIUrl}{query}");

            if (response.IsSuccessStatusCode)
            {
                WeatherInfoRoot myDeserializedClass = JsonConvert.DeserializeObject<WeatherInfoRoot>(await response.Content.ReadAsStringAsync());
                return myDeserializedClass;
            }
            else if (response.StatusCode.ToString() == "NotFound")
            {
                return "invalid city name";
            }
            return null;

        }
    }
}
