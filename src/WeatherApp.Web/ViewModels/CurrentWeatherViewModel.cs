namespace WeatherApp.Web.ViewModels
{
    public class CurrentWeatherViewModel
    {
        public string CityName { get; set; }
        public string CityDate { get; set; }
        public string CityTime { get; set; }
        public bool IsDayTime { get; set; }
        public string WeatherCondition { get; set; }
        public string Icon { get; set; }
        public int Temperature { get; set; }
    }
}
