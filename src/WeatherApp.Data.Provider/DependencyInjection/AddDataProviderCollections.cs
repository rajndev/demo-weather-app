using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Data.Provider.DataContext;
using WeatherApp.Data.Provider.Interfaces;

namespace WeatherApp.Data.Provider.DependencyInjection
{
    public static class AddDataProviderServiceCollections
    {
        public static IServiceCollection AddDataProviderDependencies(this IServiceCollection services, IConfiguration config)
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
