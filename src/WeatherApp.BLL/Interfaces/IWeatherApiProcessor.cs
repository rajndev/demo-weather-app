using System.Threading.Tasks;
using WeatherApp.BLL.Models;

namespace WeatherApp.BLL.Interfaces
{
    public interface IWeatherApiProcessor
    {
        string ApiKey { get; }

        Task<int> CallWeatherApi(string query);

        public WeatherInfoRoot GetApiResponseData();
    }
}