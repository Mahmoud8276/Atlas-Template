using Atlas.Template.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class ApplicationDbContext
    {

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            if(string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");


            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }
    }
}
