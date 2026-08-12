using Atlas.Template.Core.Interfaces;
using Atlas.Template.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class Repositories
    {
        public static IServiceCollection AddRepositoriesConfigurations(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
