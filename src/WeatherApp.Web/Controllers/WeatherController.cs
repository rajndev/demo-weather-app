using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                ProviderResult<WeatherData> apiResponseDto;

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
        {
            CurrentWeatherViewModel errorViewModel = new();

            if (TempData["isCityNameEmpty"] != null && (bool)TempData["isCityNameEmpty"])
            {
                errorViewModel.ViewErrorToken = "Invalid city name";
                return View(errorViewModel);
            }

            TempData.Keep("Weather_Info");
            var storedResults = TempData["Weather_Info"].ToString();

            var apiResponseDto = JsonConvert.DeserializeObject<ProviderResult<WeatherData>>(storedResults);

            if (apiResponseDto.StatusCode == (int)StatusCodes.NotFound)
            {
                errorViewModel.ViewErrorToken = "Invalid city name";
                return View(errorViewModel);
            }
            else if (apiResponseDto.StatusCode == (int)StatusCodes.ServiceUnavailable)
            {
                errorViewModel.ViewErrorToken = "API service unavailable";
                return View(errorViewModel);
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