using System.Collections.Generic;

namespace WeatherApp.Common.Models
{
    public class WeatherData
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string TimeZone { get; set; }
        public Current Current { get; set; }
        public List<DailyDayForecat> Daily { get; set; }
        public string CityName { get; set; }
        public string CityDay { get; set; }
        public string CityMonth { get; set; }
        public string CityDate { get; set; }
        public string CityYear { get; set; }
        public string CityTime { get; set; }
        public bool IsDayTime { get; set; }
    }
}