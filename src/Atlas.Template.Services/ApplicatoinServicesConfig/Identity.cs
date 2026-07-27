using Atlas.Template.Core.Models;
using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class Identity
    {
        public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>(options =>
            {

            }).AddRoles<UserRole>()
              .AddEntityFrameworkStores<AppDbContext>();


            return services;
        }
    }
}
