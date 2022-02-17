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
            services.Configure<ApiOptions>(options => { options.GoogleApiKey = config.GetValue<string>("GoogleGeoCodingApiOptions:ApiKey"); options.OpenWeatherMapApiKey = config.GetValue<string>("OpenWeatherMapApiOptions:ApiKey"); });

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