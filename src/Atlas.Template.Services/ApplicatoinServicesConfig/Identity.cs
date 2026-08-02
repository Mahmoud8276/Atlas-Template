using Atlas.Template.Core.Models;
using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class Identity
    {
        public static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, UserRole>(options =>
            {

            }).AddRoles<UserRole>()
              .AddEntityFrameworkStores<AppDbContext>()
              .AddSignInManager()
              .AddDefaultTokenProviders();


            return services;
        }
    }
}
