using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using WeatherApp.ApiClient.Interfaces;

namespace WeatherApp.ApiClient.DependencyInjection
{
    public static class AddApiClientServiceCollections
    {
        public static IServiceCollection AddApiClientDependencies(this IServiceCollection services, IConfiguration config)
        {
            var apiKey = config.GetSection("OpenWeatherMapApiOptions:ApiKey").Value;
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
