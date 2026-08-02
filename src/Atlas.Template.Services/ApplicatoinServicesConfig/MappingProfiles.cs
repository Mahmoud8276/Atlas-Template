using Atlas.Template.Core.Dtos.AppUserDtos;
using Atlas.Template.Core.Models;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Template.Services.ApplicatoinServicesConfig
{
    public static class MappingProfiles
    {
        public static IServiceCollection AddMappingProfiles(
            this IServiceCollection services,
            IConfiguration config)
        {
            var BaseUrl = config["Storage:BaseUrl"];


            TypeAdapterConfig<AppUser, AppUserDetailsDto>
                .NewConfig()
                .Map(dest => dest.ImageUrl, src => $"{BaseUrl}/files/UserImages/{src.Image}");

            return services;
        }
    }
}
