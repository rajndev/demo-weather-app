using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Common.Models
{
    public class DailyDayForecat
    {
        public long Dt { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
        public long Moonrise { get; set; }
        public long Moonset { get; set; }
        public Temp Temp { get; set; }
        public FeelsLike Feels_like { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double Wind_speed { get; set; }
        public List<Weather> Weather { get; set; }
    }
}