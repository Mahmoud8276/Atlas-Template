using Atlas.Template.Core.Models;
using Atlas.Template.Services.ServiceResponses;
using System.Threading.Tasks;

namespace Atlas.Template.Services.IServices
{
    public interface IWriteService<TModel, TKey, TCreateDto, TUpdateDto> 
        where TModel : BaseModel<TKey>
        where TCreateDto : class
        where TUpdateDto : class
    {
        public Task<Response> CreateAsync(TCreateDto dto);
        public Task<Response> UpdateAsync(TUpdateDto dto, TKey id);
        public Task<Response> DeleteAsync(TKey id);
    }
}
