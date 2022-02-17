namespace WeatherApp.Common.Models
{
    public class Current
    {
        public double Temp { get; set; }
        public List<Weather> Weather { get; set; }
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
        public long Dt { get; set; }
    }
}