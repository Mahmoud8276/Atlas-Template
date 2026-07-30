using Atlas.Template.Core.Dtos;
using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Interfaces.IRepositories;
using Atlas.Template.Core.Interfaces.ISpecificationParams;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.ServiceResponses;
using Mapster;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Services
{
    public abstract class ReadService<TModel, TKey, TSpecificationParams, TDetailsDto> :
        IReadService<TModel, TKey, TSpecificationParams>
        where TModel : BaseModel<TKey>
        where TDetailsDto : class
        where TSpecificationParams : IBaseSpecParams
    {

        protected readonly IGenericRepository<TModel, TKey> _repository;
        protected ReadService(IGenericRepository<TModel, TKey> repository)
        {
            _repository = repository;
        }

        protected abstract ISpecification<TModel, TKey> BuildSpec(TSpecificationParams specParams, bool isCountQuery);


        public async Task<Response> GetAllAsync(TSpecificationParams specParams)
        {
            var spec = BuildSpec(specParams, false);
            var data = await _repository.GetAllWithSpecAsync(spec);

            var countSpec = BuildSpec(specParams, true);
            var count = await _repository.GetCountWithSpecAsync(countSpec);

            var pagination = new Pagination(specParams.PageIndex, specParams.PageSize, count, data.Adapt<List<TDetailsDto>>());

            return Response.Success(pagination);
        }

        public async Task<Response> GetByIdAsync(TKey id)
        {
            var resource = await _repository.GetByIdAsync(id);
            if(resource == null)
            {
                return Response.Fail(message: "resource not found", statusCode: HttpStatusCode.NotFound);
            }

            return Response.Success(data: resource.Adapt<TDetailsDto>(), statusCode:HttpStatusCode.OK);
        }
    }
}
