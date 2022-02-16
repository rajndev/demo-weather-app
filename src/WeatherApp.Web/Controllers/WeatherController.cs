using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Common.Models;
using WeatherApp.Data.Provider.DataContext;
using WeatherApp.Provider.Interfaces;
using WeatherApp.Web.HelperClasses;
using WeatherApp.Web.ViewModels;

namespace WeatherApp.Web.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public WeatherController(IWeatherService weatherService, IMapper mapper, ApplicationDbContext context)
        {
            _weatherService = weatherService;
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
                WeatherResult<WeatherData> apiResponseDto;

                if (string.IsNullOrWhiteSpace(cityName))
                {
                    TempData["isCityNameEmpty"] = true;
                }
                else
                {
                    apiResponseDto = await _weatherService.GetCurrentWeather(cityName);
                    TempData["Weather_Info"] = JsonConvert.SerializeObject(apiResponseDto);
                }

                return RedirectToAction("ShowWeatherResponse");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ShowWeatherResponse()
        {//todo: test to see if empty city name throws proper error message
            /*TempData["isCityNameEmpty"] != null && */

            if (TempData["isCityNameEmpty"] != null && (bool)TempData["isCityNameEmpty"])
            {
                ViewData["TextResponse"] = "Invalid city name";
                return View();
            }

            TempData.Keep("Weather_Info");
            var storedResults = TempData["Weather_Info"].ToString();

            var apiResponseDto = JsonConvert.DeserializeObject<WeatherResult<WeatherData>>(storedResults);

            if ((int)apiResponseDto.StatusCode == (int)StatusCodes.NotFound)
            {
                ViewData["TextResponse"] = "Invalid city name";
                return View();
            }
            else if ((int)apiResponseDto.StatusCode == (int)StatusCodes.ServiceUnavailable)
            {
                ViewData["TextResponse"] = "API service unavailable";
                return View();
            }
            else
            {
                var currentWeatherViewModel = _mapper.Map<CurrentWeatherViewModel>(apiResponseDto);
                return View(currentWeatherViewModel);
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