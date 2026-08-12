using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddApplicationDbContextConfigurations(config);
            services.AddIdentityConfigurations();
            services.AddDataSeedersConfigurations();
            services.AddCustomServicesConfigurations();
            services.AddRepositoriesConfigurations();
            services.AddMappingProfilesConfigurations(config);
            services.AddOptionsConfigurations(config);
            services.AddAuthenticationAndAuthorizationConfigurations(config);


            return services;
        }
    }
}
