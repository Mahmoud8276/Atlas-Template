using Atlas.Template.Core.Interfaces;
using Atlas.Template.Infrastructure.DataSeeders;
using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class DataSeeders
    {
        public static IServiceCollection AddDataSeedersConfigurations(this IServiceCollection serviceCollection)
        {
            var infrastructureAssembly = typeof(AppDbContext).Assembly;
            var assimblyName = infrastructureAssembly.GetName().Name;

            var seederTypes = infrastructureAssembly
                                  .GetTypes()
                                  .Where(t => typeof(IDataSeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in seederTypes)
                serviceCollection.AddScoped(typeof(IDataSeeder), type);

            serviceCollection.AddScoped<DataSeedersRunner>();

            return serviceCollection;
        }

    }
}
