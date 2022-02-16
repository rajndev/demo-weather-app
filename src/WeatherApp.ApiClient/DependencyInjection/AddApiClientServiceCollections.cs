using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.DependencyInjection
{
    public static class AddApiClientServiceCollections
    {
        public static IServiceCollection AddApiClientDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<OpenWeatherMapApiOptions>(opt => config.GetSection("OpenWeatherMapApiOptions"));

            var apiHost = config.GetSection("OpenWeatherMapApiOptions:ApiHost").Value;

            services.AddRefitClient<IOpenWeatherAppApiService>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(apiHost);
                });

            return services;
        }
    }
}
