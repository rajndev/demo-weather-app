using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Provider.Interfaces;

namespace WeatherApp.Provider.DependencyInjection
{
    public static class AddProviderServiceCollections
    {
        public static IServiceCollection AddProviderDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherService, WeatherServiceProvider>();

            return services;
        }
    }
}