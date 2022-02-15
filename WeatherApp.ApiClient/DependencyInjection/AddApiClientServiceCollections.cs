using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WeatherApp.ApiClient.DependencyInjection
{
    public static class AddApiClientServiceCollections
    {
        public static IServiceCollection AddApiClientDependencies(this IServiceCollection services, IConfiguration config)
        {
            var apiOptions = config.GetSection("OpenWeatherMapApiOptions");
            return services;
        }
    }
}
