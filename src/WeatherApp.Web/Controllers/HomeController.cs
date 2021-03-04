using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using WeatherApp.BLL.HelperClasses;
using WeatherApp.BLL.Interfaces;
using WeatherApp.Models;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWeatherService _weatherService;
        private readonly IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IWeatherService weatherService, IConfiguration config)
        {
            _logger = logger;
            _weatherService = weatherService;
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WeatherSearch(string cityName)
        {
            if (ModelState.IsValid)
            {
                object weatherInfoRoot;
                var apiKey = _config.GetValue<string>("AccuweatherAPIKey");

                if (cityName != null)
                {
                    if (cityName.Contains(","))
                    {
                        var split = cityName.Split(",");
                        weatherInfoRoot = await _weatherService.GetCurrentWeather(split[0], apiKey);
                    }
                    else
                    {
                        weatherInfoRoot = await _weatherService.GetCurrentWeather(cityName, apiKey);
                    }
                }
                else
                {
                    TempData["Weather_Info"] = "invalid city name";
                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }

                if (weatherInfoRoot.GetType() == typeof(string))
                {
                    TempData["Weather_Info"] = weatherInfoRoot.ToString();
                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }
                else
                {

                    TempData["Weather_Info"] = JsonConvert.SerializeObject(weatherInfoRoot);

                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult CurrentWeather()
        {
            TempData.Keep("Weather_Info");
            string storedResults = TempData["Weather_Info"].ToString();

            if (storedResults == "invalid city name")
            {
                ViewData["TextResponse"] = "invalid city name";
                return View();
            }
            else if (storedResults == "service unavailable")
            {
                ViewData["TextResponse"] = "service unavailable";
                return View();
            }
            else
            {
                WeatherInfoRoot weatherInfoRoot = JsonConvert.DeserializeObject<WeatherInfoRoot>(storedResults);
                ViewData["IsWeatherInfoNull"] = weatherInfoRoot == null ? "true" : "false";
                return View(weatherInfoRoot);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
