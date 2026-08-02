using Atlas.Template.Services.IServices;
using Atlas.Template.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class AddCustomServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            // Ex:
            // services.AddScoped<IProductService, ProductService>();
             services.AddScoped<ITokenService, TokenService>();
             services.AddScoped<IAccountService, AccountService>();
             services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
