namespace WeatherApp.Common.Models
{
    public class OpenWeatherMapApiOptions
    {
        public const string Position = "OpenWeatherMapApiOptions";
        public string ApiKey { get; set; } = string.Empty;
        public string ApiHost { get; set; } = string.Empty;
    }
}