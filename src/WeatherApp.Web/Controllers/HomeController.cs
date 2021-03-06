using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Models;
using WeatherApp.DAL.Data;
using WeatherApp.Models;
using WeatherApp.Web.ViewModels;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public HomeController(IWeatherService weatherService, IConfiguration config, IMapper mapper, ApplicationDbContext context)
        {
            _weatherService = weatherService;
            _config = config;
            _mapper = mapper;
            _context = context;
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
                object weatherInfoDTO;
                var apiKey = _config.GetValue<string>("OpenWeatherMapAPIKey");

                if (cityName != null)
                {
                    if (cityName.Contains(","))
                    {
                        var split = cityName.Split(",");
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(split[0], apiKey);
                    }
                    else
                    {
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(cityName, apiKey);
                    }
                }
                else
                {
                    TempData["Weather_Info"] = "invalid city name";
                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }

                if (weatherInfoDTO.GetType() == typeof(string))
                {
                    TempData["Weather_Info"] = weatherInfoDTO.ToString();
                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }
                else
                {
                    TempData["Weather_Info"] = JsonConvert.SerializeObject(weatherInfoDTO);
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
            else if (storedResults == "api service unavailable")
            {
                ViewData["TextResponse"] = "api service unavailable";
                return View();
            }
            else
            {
                WeatherInfoDTO weatherInfoDTO = JsonConvert.DeserializeObject<WeatherInfoDTO>(storedResults);
                var currentWeatherViewModel = _mapper.Map<CurrentWeatherViewModel>(weatherInfoDTO);
                ViewData["IsWeatherInfoNull"] = weatherInfoDTO == null ? "true" : "false";
                return View(currentWeatherViewModel);
            }
        }

        [HttpPost]
        public JsonResult GetAutocompleteList(string cityName)
        {
            var cityNameList = _context.Cities.Where(s => s.Name.Contains(cityName)).Take(8).Select(p => new { p.Name, p.Id }).ToList();

            //string[] cityNameList = { "homer", "bart", "lisa" };
            return Json(cityNameList);
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
