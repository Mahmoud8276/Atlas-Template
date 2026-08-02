using Atlas.Template.Core.Dtos.AccountDtos;
using Atlas.Template.Services.Responses;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IAccountService
    {
        public Task<Response> RegisterUserAsync(RegisterDto dto);
        public Task<Response> LoginUserAsync(LoginDto dto);
        public Task<Response> ConfirmEmailAsync(string userId, string token);
    }
}
