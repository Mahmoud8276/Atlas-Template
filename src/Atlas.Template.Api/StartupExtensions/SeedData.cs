using Atlas.Template.Infrastructure.DataSeeders;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Atlas.Template.Api.StartupExtensions
{
    public static class SeedData
    {
        public static async Task SeedDataAsync(this IServiceScope scope)
        {
            var runner = scope.ServiceProvider.GetRequiredService<DataSeedersRunner>();
            await runner.RunAsync();
        }

    }
}
