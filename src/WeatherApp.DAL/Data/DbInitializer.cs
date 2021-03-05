using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.DAL.Entities;
using WeatherApp.DAL.Interfaces;

namespace WeatherApp.DAL.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DbInitializer(IServiceScopeFactory scopeFactory, IWebHostEnvironment environment)
        {
            _scopeFactory = scopeFactory;
            _hostEnvironment = environment;
        }

        public void Initialize()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    context.Database.EnsureCreatedAsync();
                }
            }
        }

        public async Task SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
                {
                    if (!context.Cities.Any())
                    {
                        string path = Path.Combine(_hostEnvironment.WebRootPath, "citylist.json");
                        using (StreamReader reader = new StreamReader(path))
                        {
                            string jsonData = reader.ReadToEnd();
                            List<City> cityList = JsonConvert.DeserializeObject<List<City>>(jsonData);
                            await context.Cities.AddRangeAsync(cityList);
                            await context.SaveChangesAsync();
                        }
                    }
                }
            }
        }
    }
}