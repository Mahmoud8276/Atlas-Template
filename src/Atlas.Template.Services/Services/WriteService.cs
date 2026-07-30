using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Interfaces.IRepositories;
using Atlas.Template.Core.Interfaces.ISpecificationParams;
using Atlas.Template.Core.Models;
using Atlas.Template.Services.IServices;
using Atlas.Template.Services.ServiceResponses;
using FluentValidation;
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
                        statusCode: HttpStatusCode.BadRequest);
                }
            }

            var entity = dto.Adapt<TModel>();

            await BeforeCreateAsync(entity, dto);

            await _repository.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterCreateAsync(entity, dto);

            return Response.Success(data: entity.Adapt<TDetailsDto>(), statusCode: HttpStatusCode.Created);
        }

        public async Task<Response> UpdateAsync(TUpdateDto dto, TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.Fail(message: "Resource not found.", statusCode: HttpStatusCode.NotFound);

            if (_updateValidator is not null)
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Response.Fail(
                        message: "Validation failed.",
                        details: string.Join("; ", validationResult.Errors),
                        statusCode: HttpStatusCode.BadRequest);
                }
            }

            await BeforeUpdateAsync(entity, dto);

            dto.Adapt(entity);

            _repository.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterUpdateAsync(entity, dto);

            return Response.Success(data: entity.Adapt<TDetailsDto>(), statusCode: HttpStatusCode.OK);
        }

        public async Task<Response> DeleteAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity is null)
                return Response.Fail(message: "Resource not found.", statusCode: HttpStatusCode.NotFound);

            await BeforeDeleteAsync(entity);

            _repository.DeleteAsync(entity);
            await _unitOfWork.CompleteAsync();

            await AfterDeleteAsync(entity);

            return Response.Success(data: null, message: "Deleted successfully.", statusCode: HttpStatusCode.OK);
        }



        protected virtual Task BeforeCreateAsync(TModel entity, TCreateDto dto) => Task.CompletedTask;
        protected virtual Task AfterCreateAsync(TModel entity, TCreateDto dto) => Task.CompletedTask;
        protected virtual Task BeforeUpdateAsync(TModel entity, TUpdateDto dto) => Task.CompletedTask;
        protected virtual Task AfterUpdateAsync(TModel entity, TUpdateDto dto) => Task.CompletedTask;
        protected virtual Task BeforeDeleteAsync(TModel entity) => Task.CompletedTask;
        protected virtual Task AfterDeleteAsync(TModel entity) => Task.CompletedTask;
    }
}

