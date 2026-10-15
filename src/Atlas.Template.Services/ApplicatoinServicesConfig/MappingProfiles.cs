using Atlas.Template.Core.Dtos.AppUserDtos;
using Atlas.Template.Core.Models;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class MappingProfiles
    {
        public static IServiceCollection AddMappingProfilesConfigurations(
            this IServiceCollection services,
            IConfiguration config)
        {
            Configure(config["Storage:BaseUrl"]);

            return services;
        }

        public static void Configure(string? baseUrl)
        {
            TypeAdapterConfig<AppUser, UserDetailsDto>
                .NewConfig()
                .Map(dest => dest.ImageUrl, src => $"{baseUrl}/files/UserImages/{src.Image}");
        }
    }
}
