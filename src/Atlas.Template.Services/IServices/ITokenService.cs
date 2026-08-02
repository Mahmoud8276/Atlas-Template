using Atlas.Template.Core.Models;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessTokenAsync(AppUser user);
    }
}
