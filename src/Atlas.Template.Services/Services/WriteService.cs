using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Interfaces.IRepositories;
using Atlas.Template.Core.Interfaces.ISpecificationParams;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.Responses;
using FluentValidation;
using Mapster;
using System.Net;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Services
{
    public abstract class WriteService<TModel, TKey, TSpecificationParams, TCreateDto, TUpdateDto, TDetailsDto>
        : ReadService<TModel, TKey, TSpecificationParams, TDetailsDto>,
            IWriteService<TModel, TKey, TCreateDto, TUpdateDto>
        where TModel : BaseModel<TKey>
        where TDetailsDto : class
        where TCreateDto : class
        where TUpdateDto : class
        where TSpecificationParams : IBaseSpecParams
    {
        protected readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<TCreateDto>? _createValidator;
        private readonly IValidator<TUpdateDto>? _updateValidator;

        public WriteService(
            IGenericRepository<TModel, TKey> repository,
            IUnitOfWork unitOfWork,
            IValidator<TCreateDto>? createValidator = null,
            IValidator<TUpdateDto>? updateValidator = null)
            : base(repository)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Response> CreateAsync(TCreateDto dto)
        {
            if (_createValidator is not null)
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Response.Fail(
                        message: "Validation failed.",
                        details: string.Join("; ", validationResult.Errors),
                        statusCode: (int)HttpStatusCode.BadRequest);
                }
            }

            var entity = dto.Adapt<TModel>();

            var result = await BeforeCreateAsync(entity, dto);
            if (!result.IsSuccess)
                return result;


            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterCreateAsync(entity, dto);

            return Response.Success(data: entity.Adapt<TDetailsDto>(), statusCode: (int)HttpStatusCode.Created);
        }

        public async Task<Response> UpdateAsync(TUpdateDto dto, TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.Fail(message: "Resource not found.", statusCode: (int)HttpStatusCode.NotFound);

            if (_updateValidator is not null)
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Response.Fail(
                        message: "Validation failed.",
                        details: string.Join("; ", validationResult.Errors),
                        statusCode: (int)HttpStatusCode.BadRequest);
                }
            }

            var result = await BeforeUpdateAsync(entity, dto);
            if (!result.IsSuccess)
                return result;

            dto.Adapt(entity);

            _repository.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterUpdateAsync(entity, dto);

            return Response.Success(data: entity.Adapt<TDetailsDto>(), statusCode: (int)HttpStatusCode.OK);
        }

        public async Task<Response> DeleteAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.Fail(message: "Resource not found.", statusCode: (int)HttpStatusCode.NotFound);

            var result = await BeforeDeleteAsync(entity);
            if(!result.IsSuccess)
                return result;

            _repository.DeleteAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterDeleteAsync(entity);

            return Response.Success(data: null, message: "Deleted successfully.", statusCode: (int)HttpStatusCode.OK);
        }



        protected virtual async Task<Response> BeforeCreateAsync(TModel entity, TCreateDto dto) => Response.Success();
        protected virtual Task AfterCreateAsync(TModel entity, TCreateDto dto) => Task.CompletedTask;
        protected virtual async Task<Response> BeforeUpdateAsync(TModel entity, TUpdateDto dto) => Response.Success();
        protected virtual  Task AfterUpdateAsync(TModel entity, TUpdateDto dto) => Task.CompletedTask;
        protected virtual async Task<Response> BeforeDeleteAsync(TModel entity) => Response.Success();
        protected virtual Task AfterDeleteAsync(TModel entity) => Task.CompletedTask;
    }
}

