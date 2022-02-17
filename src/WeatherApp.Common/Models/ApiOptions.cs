namespace WeatherApp.Common.Models
{
    public class ApiOptions
    {
        public string OpenWeatherMapApiKey { get; set; } = string.Empty;
        public string GoogleApiKey { get; set; }
        public string ApiHost { get; set; } = string.Empty;
    }
}