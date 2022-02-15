using Microsoft.Extensions.DependencyInjection;
using WeatherApp.BLL.Interfaces;
using WeatherApp.BLL.Services;

namespace WeatherApp.BLL.DependencyInjection
{
    public static class AddBLLServiceCollections
    {
        public static IServiceCollection AddBLLDependencies(this IServiceCollection services)
        {
            services.AddTransient<IWeatherService, WeatherService>();

            return services;
        }
    }
}
