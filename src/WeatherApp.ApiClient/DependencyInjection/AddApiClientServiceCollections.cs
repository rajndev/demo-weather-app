using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using WeatherApp.ApiClient.Interfaces;
using WeatherApp.Common.Models;

namespace WeatherApp.ApiClient.DependencyInjection
{
    public static class AddApiClientServiceCollections
    {
        public static IServiceCollection AddApiClientDependencies(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiOptions>(options => { options.GoogleApiKey = config.GetValue<string>("ApiOptions:GoogleApiKey"); options.OpenWeatherMapApiKey = config.GetValue<string>("ApiOptions:OpenWeatherMapApiKey"); });

            var apiHost = config.GetSection("OpenWeatherMapApiOptions:ApiHost").Value;

            services.AddRefitClient<IOpenWeatherMapApiService>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(apiHost);
                });

            return services;
        }
    }
}