using Atlas.Template.Services.IServices;
using Atlas.Template.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class CustomServices
    {
        public static IServiceCollection AddCustomServicesConfigurations(this IServiceCollection services)
        {

             services.AddScoped<ITokenService, TokenService>();
             services.AddScoped<IAccountService, AccountService>();
             services.AddScoped<IEmailService, EmailService>();
             services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
