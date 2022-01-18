﻿using AutoMapper;
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
using WeatherApp.DAL.Entities;
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
        public async Task<IActionResult> SearchWeather(
            string cityName)
        {
            if (ModelState.IsValid)
            {
                var apiKey = _config.GetValue<string>("OpenWeatherMapAPIKey");
                WeatherInfoDTO weatherInfoDTO = null;

                if (String.IsNullOrWhiteSpace(cityName))
                {
                    TempData["isCityNameEmpty"] = true;
                    return RedirectToAction("ShowWeatherResponse");
                }
                else
                {
                    var split = cityName.Split(",");
                    int? cityCode = null;

                    if (split.Length == 3)
                    {
                        cityCode = await GetCityCode(split, 3);
                    }
                    else if (split.Length == 2)
                    {
                        cityCode = await GetCityCode(split, 2);
                    }

                    if (cityCode != null)
                    {
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityName, cityId: cityCode);
                    }
                    else
                    {
                        weatherInfoDTO = await _weatherService.GetCurrentWeather(apiKey: apiKey, cityName: cityName);
                    }
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

                WeatherInfoDTO weatherInfoDTO = JsonConvert.DeserializeObject<WeatherInfoDTO>(storedResults);

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

        public async Task<int?> GetCityCode(string[] split, int length)
        {
            City cityRecord = null;

            if(length == 3)
            {
                cityRecord = await _context.Cities.Where(p => p.Name == split[0].Trim() && p.State == split[1].Trim() && p.Country == split[2].Trim()).FirstOrDefaultAsync();
            }  
            else if(length == 2)
            {
                cityRecord = await _context.Cities.Where(p => p.Name == split[0].Trim() && p.State == split[1].Trim()).FirstOrDefaultAsync();
            }

            return cityRecord?.CityCode;
        }
    }
}
