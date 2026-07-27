using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Atlas.Template.Api.StartupExtensions
{
    public static class StartupExtensions
    {
        public static async Task ExecuteStartupExtensions(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await context.Database.MigrateAsync();

            await scope.SeedDataAsync();
        }
    }
}
