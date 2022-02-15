using Microsoft.Extensions.DependencyInjection;
using WeatherApp.HelperClasses;

namespace WeatherApp.Web.DependencyInjection
{
    public static class AddControllerServiceCollections
    {
        public static IServiceCollection AddControllerDependencies(this IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAutoMapper(c => c.AddProfile<AutoMappingProfile>(), typeof(Startup));
            return services;
        }
    }
}
