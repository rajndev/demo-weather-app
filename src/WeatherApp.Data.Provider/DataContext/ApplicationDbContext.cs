using Microsoft.EntityFrameworkCore;
using WeatherApp.Data.Provider.Entities;

namespace WeatherApp.Data.Provider.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
    }
}