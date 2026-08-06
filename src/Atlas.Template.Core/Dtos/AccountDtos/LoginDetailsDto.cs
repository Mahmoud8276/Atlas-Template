using Atlas.Template.Core.Dtos.AppUserDtos;
using System;
using System.Text.Json.Serialization;

namespace Atlas.Template.Core.Dtos.AccountDtos
{
    public class LoginDetailsDto
    {
        public UserDetailsDto User { get; set; }
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
