using Atlas.Template.Core.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class Options
    {
        public static IServiceCollection AddOptionsConfigurations(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection("Email"));

            return services;
        }
    }
}
