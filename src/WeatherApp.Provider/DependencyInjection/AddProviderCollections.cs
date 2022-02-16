using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Provider.Interfaces;
using WeatherApp.Provider.Services;

namespace WeatherApp.Provider.DependencyInjection
{
    public static class AddProviderServiceCollections
    {
        public static IServiceCollection AddProviderDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherService, WeatherService>();

            return services;
        }
    }
}