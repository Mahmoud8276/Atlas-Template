using Atlas.Template.Core.Interfaces.ISpecificationParams;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.Responses;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IWriteService<TModel, TKey, TCreateDto, TUpdateDto, TSpecificationParams> 
        : IReadService<TModel, TKey, TSpecificationParams>
        where TModel : BaseModel<TKey>
        where TCreateDto : class
        where TUpdateDto : class
        where TSpecificationParams: IBaseSpecParams
    {
        public Task<Response> CreateAsync(TCreateDto dto);
        public Task<Response> UpdateAsync(TUpdateDto dto, TKey id);
        public Task<Response> DeleteAsync(TKey id);
    }
}
