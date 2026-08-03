using Atlas.Template.Services.Responses;
using Atlas.Template.Services.SpecificationParams;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IUserService
    {
        Task<Response> GetByIdAsync(string id);
        Task<Response> GetAllAsync(UserSpecParams specParams);
    }
}
