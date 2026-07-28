using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class AddCustomServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // Ex:
            // services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
