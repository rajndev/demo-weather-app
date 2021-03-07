using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
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
        public async Task<IActionResult> WeatherSearch(
            string cityName,
            string cityIdHidden,
            string cityNameHidden,
            string stateHidden,
            string countryHidden)
        {
            if (ModelState.IsValid)
            {
                var apiKey = _config.GetValue<string>("OpenWeatherMapAPIKey");
                object weatherInfoDTO = null;

                var hiddenSearchTerm = stateHidden == null ? cityNameHidden + ", " + countryHidden : cityNameHidden + ", " + stateHidden + ", " + countryHidden;

                if (cityName == null)
                {
                    TempData["Weather_Info"] = "invalid city name";
                    TempData.Keep("Weather_Info");
                    return RedirectToAction("CurrentWeather", "Home");
                }
                else if (hiddenSearchTerm == cityName)
                {
                    int id;
                    Int32.TryParse(cityIdHidden, out id);

                    weatherInfoDTO = await _weatherService.GetCurrentWeather(
                        apiKey: apiKey,
                        cityId: id,
                        cityName: cityName);
                }
                else if (hiddenSearchTerm != cityName)
                {
                    var split = cityName.Split(",");

                    if (split.Length == 3)
                    {
                        var cityRecord = await _context.Cities.Where(p => p.Name == split[0].Trim() && p.State == split[1].Trim() && p.Country == split[2].Trim()).FirstOrDefaultAsync();

                        if (cityRecord == null)
                        {
                            weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityName: cityName);
                        }
                        else
                        {
                            weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityId: cityRecord.CityCode, cityName: cityName);
                        }
                    }
                    else if (split.Length == 2)
                    {
                        int cityId = FindCityNameIdFromDb(cityName);
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityId: cityId, cityName: cityName);
                    }
                    else
                    {
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityName: cityName);
                    }
                }

                if (weatherInfoDTO is string)
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

            return RedirectToAction("Index");
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
                return View(currentWeatherViewModel);
            }
        }

        [HttpPost]
        public JsonResult GetAutocompleteList(string cityName)
        {
            var cityNameList = _context.Cities.Where(s => s.Name.Contains(cityName)).Take(8).Select(p => new { p.Name, p.State, p.Country, p.CityCode }).ToList();

            return Json(cityNameList);
        }

        private int FindCityNameIdFromDb(string cityName)
        {
            var split = cityName.Split(",");
            var caps = split[1].ToLower().Trim();

            var cityRecords = _context.Cities.Where(p => p.Name == split[0]).ToList();

            foreach (var city in cityRecords)
            {
                if (city.State.ToLower() == caps)
                    return city.CityCode;
            }

            return 0;
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
