# 04 — Read/Write Service Pattern

`ReadService` and `WriteService` do for the business layer what the generic repository does for data access: standard CRUD, written once, so a new service is mostly plumbing plus the two or three things that make *this* entity's business logic different.

## `ReadService<TModel, TKey, TSpecificationParams, TDetailsDto>`

Provides two methods, both already fully implemented:

- **`GetByIdAsync(id)`** — fetches by id, maps to `TDetailsDto`, returns a `NotFound` `Response` if it doesn't exist
- **`GetAllAsync(specParams)`** — builds a specification from `specParams`, fetches the page of data *and* a separate count of all matching rows, and returns both wrapped in a `Pagination` object

The one thing you must supply is:

```csharp
protected abstract ISpecification<TModel, TKey> BuildSpec(TSpecificationParams specParams, bool isCountQuery);
```

**Why `isCountQuery` exists:** `GetAllAsync` calls `BuildSpec` twice — once for the actual page of data (with pagination applied), and once for the total count (same filter, no pagination, since you're counting *all* matches, not just one page of them). Your `BuildSpec` implementation is expected to branch on the flag and skip `ApplyPagination` when `isCountQuery` is `true`.

## `WriteService<TModel, TKey, TSpecificationParams, TCreateDto, TUpdateDto, TDetailsDto>`

Inherits `ReadService`, adding:

- **`CreateAsync(dto)`** — validates (if a validator is registered), maps DTO → entity, persists it, maps the saved entity back to `TDetailsDto`
- **`UpdateAsync(dto, id)`** — loads the entity, returns `NotFound` if missing, validates, applies the DTO onto the existing entity, persists, maps back
- **`DeleteAsync(id)`** — loads the entity, returns `NotFound` if missing, deletes, persists

Each of these calls a pair of hooks around the actual database operation:

| Method | Before hook | After hook |
|---|---|---|
| `CreateAsync` | `BeforeCreateAsync(entity, dto)` | `AfterCreateAsync(entity, dto)` |
| `UpdateAsync` | `BeforeUpdateAsync(entity, dto)` | `AfterUpdateAsync(entity, dto)` |
| `DeleteAsync` | `BeforeDeleteAsync(entity)` | `AfterDeleteAsync(entity)` |

**Why hooks instead of just overriding the whole method:** if you only need to add one rule — say, rejecting a duplicate SKU before a `Product` is created — overriding `BeforeCreateAsync` lets you add that one rule without reimplementing validation, mapping, persistence, and response-shaping yourself. The `Before*` hooks return a `Response`; if `IsSuccess` is `false`, `WriteService` stops immediately and returns that response without touching the database. The `After*` hooks run once the save has already succeeded — useful for side effects like sending a notification, and they don't affect the response since it's already been decided.

Validators are optional constructor parameters (`IValidator<TCreateDto>?`, `IValidator<TUpdateDto>?`) — if you don't register one, `WriteService` just skips validation for that DTO. See [`docs/07-validation.md`](07-validation.md) for how to register one.

## Worked example: a `ProductService` with one custom rule

```csharp
public class ProductService
    : WriteService<Product, int, ProductSpecParams, ProductCreateDto, ProductUpdateDto, ProductDetailsDto>
{
    private readonly IGenericRepository<Product, int> _productRepository;

    public ProductService(
        IGenericRepository<Product, int> repository,
        IUnitOfWork unitOfWork,
        IValidator<ProductCreateDto>? createValidator = null)
        : base(repository, unitOfWork, createValidator)
    {
        _productRepository = repository;
    }

    protected override ISpecification<Product, int> BuildSpec(ProductSpecParams specParams, bool isCountQuery)
        => new ProductSpecification(specParams, isCountQuery);

    protected override async Task<Response> BeforeCreateAsync(Product entity, ProductCreateDto dto)
    {
        var duplicates = await _productRepository.FindAsync(p => p.Sku == dto.Sku);
        if (duplicates.Any())
            return Response.Fail(message: "A product with this SKU already exists.", statusCode: (int)HttpStatusCode.Conflict);

        return Response.Success();
    }
}
```

With this in place, `ProductService` already has working `GetAllAsync`, `GetByIdAsync`, `CreateAsync` (with the SKU check), `UpdateAsync`, and `DeleteAsync` — the only code you wrote is the spec-building method and one rule.

`ProductSpecification` here takes the `isCountQuery` flag directly (unlike the version in [`docs/03-specifications.md`](03-specifications.md), which always paginated) — this is the version you'd actually write once you're building on `ReadService`, since `BuildSpec` needs to skip pagination for the count query:

```csharp
public class ProductSpecification : BaseSpecification<Product, int>
{
    public ProductSpecification(ProductSpecParams specParams, bool isCountQuery)
        : base(p =>
            (string.IsNullOrEmpty(specParams.Category) || p.Category == specParams.Category) &&
            (!specParams.MinPrice.HasValue || p.Price >= specParams.MinPrice))
    {
        AddOrderBy(p => p.Name);

        if (!isCountQuery)
            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
    }
}
```

## When *not* to use this pattern

`AccountService` and `UserService` don't inherit `ReadService`/`WriteService` at all — they work directly against `UserManager<AppUser>`, because ASP.NET Core Identity already owns CRUD for users and has its own rules (password hashing, lockouts, role management) that don't map cleanly onto generic create/update/delete. If an entity is managed by something other than the generic repository, write the service by hand instead of forcing it through this pattern.

---

**Next:** [05 — Authentication & JWT](05-authentication-and-jwt.md)