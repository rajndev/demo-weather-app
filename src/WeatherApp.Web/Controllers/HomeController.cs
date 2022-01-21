using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Models;
using WeatherApp.DAL.Data;
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
        public async Task<IActionResult> SearchWeather(string cityName)
        {
            if (ModelState.IsValid)
            {
                var apiKey = _config.GetValue<string>("OpenWeatherMapAPIKey");
                WeatherInfoDto weatherInfoDTO;

                if (String.IsNullOrWhiteSpace(cityName))
                {
                    TempData["isCityNameEmpty"] = true;
                    return RedirectToAction("ShowWeatherResponse");
                }
                else
                {
                    weatherInfoDTO = await _weatherService.GetCurrentWeather(cityName, apiKey);
                }

                TempData["Weather_Info"] = JsonConvert.SerializeObject(weatherInfoDTO);
                TempData.Keep("Weather_Info");
                return RedirectToAction("ShowWeatherResponse");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ShowWeatherResponse()
        {
            if(TempData["isCityNameEmpty"] != null && (bool)TempData["isCityNameEmpty"])
            {
                ViewData["TextResponse"] = "Invalid city name";
                return View();
            }
            else
            {
                TempData.Keep("Weather_Info");
                var storedResults = TempData["Weather_Info"].ToString();

                WeatherInfoDto weatherInfoDTO = JsonConvert.DeserializeObject<WeatherInfoDto>(storedResults);

                if (weatherInfoDTO.isStatusNotFound)
                {
                    ViewData["TextResponse"] = "Invalid city name";
                    return View();
                }
                else if (weatherInfoDTO.isStatusOther)
                {
                    ViewData["TextResponse"] = "API service unavailable";
                    return View();
                }
                else
                {
                    var currentWeatherViewModel = _mapper.Map<CurrentWeatherViewModel>(weatherInfoDTO);
                    return View(currentWeatherViewModel);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetAutocompleteList(string cityName)
        {
            var cityNameList = await _context.Cities.Where(s => s.Name.Contains(cityName)).Take(8).Select(p => new { p.Name, p.State, p.Country, p.CityCode }).ToListAsync();

            return Json(cityNameList);
        }

        
    }
}
