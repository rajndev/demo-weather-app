using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp.Common.Models
{
    public class Temp
    {
        public double Day { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Night { get; set; }
        public double Eve { get; set; }
        public double Morn { get; set; }
    }
}