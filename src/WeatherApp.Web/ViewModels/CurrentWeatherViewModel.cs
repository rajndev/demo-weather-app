using System.Collections.Generic;
using WeatherApp.Common.Models;

namespace WeatherApp.Web.ViewModels
{
    public class CurrentWeatherViewModel
    {
        public long Dt { get; set; }
        public string TimeZone { get; set; }
        public string CityName { get; set; }
        public string CityDay { get; set; }
        public string CityMonth { get; set; }
        public string CityDate { get; set; }
        public string CityYear { get; set; }
        public string CityTime { get; set; }
        public bool IsDayTime { get; set; }
        public string WeatherCondition { get; set; }
        public string Icon { get; set; }
        public int Temperature { get; set; }
        public string ViewErrorToken { get; set; }
        public List<DailyDayForecat> Daily { get; set; }
    }
}