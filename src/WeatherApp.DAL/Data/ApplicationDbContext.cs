using Microsoft.EntityFrameworkCore;
using WeatherApp.DAL.Entities;

namespace WeatherApp.DAL.Data
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