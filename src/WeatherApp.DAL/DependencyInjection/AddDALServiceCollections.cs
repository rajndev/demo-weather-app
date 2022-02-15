using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.DAL.Data;
using WeatherApp.DAL.Interfaces;

namespace WeatherApp.DAL.DependencyInjection
{
    public static class AddDALServiceCollections
    {
        public static IServiceCollection AddDALDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(
                                config.GetConnectionString("DefaultConnection")),
                                ServiceLifetime.Scoped
                                );

            services.AddTransient<IDbInitializer, DbInitializer>();

            return services;
        }
    }
}
