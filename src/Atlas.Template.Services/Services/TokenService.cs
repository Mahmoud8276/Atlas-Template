using Atlas.Template.Core.Models;
using Atlas.Template.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Atlas.Template.Core.Options;

namespace Atlas.Template.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(
            IOptions<JwtOptions> jwtOptions,
            UserManager<AppUser> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;
        }

        public async Task<string> GenerateAccessTokenAsync(AppUser user)
        {
            var ExpirationDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtOptions.ExpirationInMinutes));
            var Audience = _jwtOptions.Audience;
            var Issuer = _jwtOptions.Issuer;
            var Key = _jwtOptions.Key;
            var UserRoles = await _userManager.GetRolesAsync(user);

            var Claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, Audience),
                new Claim(JwtRegisteredClaimNames.Iss, Issuer),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            Claims.AddRange(UserRoles.Select(role=> new Claim(ClaimTypes.Role, role)));

            var HashKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Key));
            var SigningCredintials = new SigningCredentials(HashKey, SecurityAlgorithms.HmacSha256);

            var tokenGenerator = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: Claims,
                expires: ExpirationDate,
                signingCredentials: SigningCredintials);

            return new JwtSecurityTokenHandler().WriteToken(tokenGenerator);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(Convert.ToDouble(_jwtOptions.RefreshTokenExpirationInDays))
            };
        }
    }
}
