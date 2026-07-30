using Atlas.Template.Core.Models;
using Atlas.Template.Services.ServiceResponses;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IReadService<TModel, TKey, TSpecificationParams> 
        where TModel : BaseModel<TKey>
    {
        public Task<Response> GetByIdAsync(TKey id);

        public Task<Response> GetAllAsync(TSpecificationParams specParams);
    }
}
