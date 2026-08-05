using Atlas.Template.Core.Dtos.AccountDtos;
using Atlas.Template.Services.Responses;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IAccountService
    {
        public Task<Response> RegisterUserAsync(RegisterDto dto);
        public Task<Response<LoginDetailsDto>> LoginUserAsync(LoginDto dto);
        public Task<Response> ConfirmEmailAsync(string userId, string token);
        public Task<Response> ForgetPasswordAsync(ForgetPasswordDto dto);
        public Task<Response> ResetPasswordAsync(ResetPasswordDto dto);
        public Task<Response<LoginDetailsDto>> RefreshTokenAsync(string refreshToken);
        public Task<Response> RevokeRefreshTokenAsync(string refreshToken);
    }
}
