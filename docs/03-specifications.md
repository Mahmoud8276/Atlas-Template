# 03 — Specifications

The generic repository (see [`docs/02`](02-generic-repository-and-uow.md)) covers basic CRUD, but real endpoints usually need filtering, sorting, eager-loading related data, and pagination — and that query logic shouldn't be scattered as raw LINQ across your services. The specification pattern gives that logic a home: a single object that describes *what* you want to query, separate from the code that turns it into an actual EF Core query.

## `ISpecification<TModel, TKey>`

Defined in Core. Every specification describes a query in five parts:

- **`Criteria`** — a `Where`-style filter expression
- **`Includes` / `StringIncludes`** — related entities to eager-load, either as a strongly-typed expression (`x => x.Category`) or a string path for deeper chains (`"Category.Store"`)
- **`OrderBy` / `OrderByDesc`** — a single sort expression (only one is applied — see below)
- **`Skip` / `Take` / `IsPagination`** — paging bounds, applied only when `IsPagination` is true

## `BaseSpecification<TModel, TKey>`

Defined in Services. You don't implement `ISpecification` by hand — you inherit `BaseSpecification` and use its protected helper methods to build the spec, usually from the constructor:

- **`AddInclude(expression)`** / **`AddInclude(string)`**
- **`AddOrderBy(expression)`** / **`AddOrderByDesc(expression)`**
- **`ApplyPagination(skip, take)`**

Keeping these `protected` (rather than public settable properties) means a specification is built once, in one place, and can't be mutated after the fact by whatever code happens to receive it.

## `SpecificationEvaluator`

Defined in Infrastructure. Takes an `IQueryable<TModel>` and an `ISpecification`, and applies each part **in a fixed order**:

1. `Where(spec.Criteria)` — if set
2. `OrderBy(spec.OrderBy)`, else `OrderByDescending(spec.OrderByDesc)` — only one runs; `OrderBy` wins if both happen to be set
3. `Include(...)` for every expression in `Includes`, then every string in `StringIncludes`
4. `Skip(spec.Skip).Take(spec.Take)` — only if `IsPagination` is true

This is the piece the generic repository calls internally for `GetWithSpecAsync`, `GetAllWithSpecAsync`, and `GetCountWithSpecAsync` — you never call `SpecificationEvaluator` directly from a service.

## Spec params: turning query-string input into a specification

`IBaseSpecParams`/`BaseSpecParams` (Services layer) model the paging input a controller receives from the query string:

- `PageIndex` — defaults to 1
- `PageSize` — defaults to 10, and is silently clamped to a maximum of 15 if the caller asks for more

Extend `BaseSpecParams` per entity to add filter fields, the way `UserSpecParams` adds a `UserName` filter:

```csharp
public class ProductSpecParams : BaseSpecParams
{
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
}
```

**Being upfront about the current state:** `UserSpecParams` exists and is used by `UserController`/`UserService`, but `UserService.GetAllAsync` builds its query with manual `Where`/`Skip`/`Take` directly against `UserManager.Users` — it doesn't go through `ISpecification`/`BaseSpecification`/`SpecificationEvaluator` at all, because `AppUser` is managed by ASP.NET Core Identity rather than the generic repository. So today, the specification pattern itself has no concrete example wired up anywhere in the template — it's ready to use, but you'll be the first to write a real subclass of `BaseSpecification`. The worked example below is exactly that.

## Worked example: a `ProductSpecification`

```csharp
public class ProductSpecification : BaseSpecification<Product, int>
{
    public ProductSpecification(ProductSpecParams specParams)
        : base(p =>
            (string.IsNullOrEmpty(specParams.Category) || p.Category == specParams.Category) &&
            (!specParams.MinPrice.HasValue || p.Price >= specParams.MinPrice))
    {
        AddOrderBy(p => p.Name);
        ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
    }
}
```

Using it from a service, combined with `GetCountWithSpecAsync` for the pagination total:

```csharp
public async Task<Response<Pagination>> GetAllAsync(ProductSpecParams specParams)
{
    var spec = new ProductSpecification(specParams);

    var products = await _repository.GetAllWithSpecAsync(spec);
    var count = await _repository.GetCountWithSpecAsync(spec);

    var pagination = new Pagination(
        specParams.PageIndex,
        specParams.PageSize,
        count,
        products.Adapt<List<ProductDetailsDto>>());

    return Response<Pagination>.Success(pagination);
}
```

Note that `GetCountWithSpecAsync` is called with the *same* spec — it runs the `Criteria` filter but ignores paging, so you get the total number of matching rows, not just the page size.

---

**Next:** [04 — Read/Write Service Pattern](04-read-write-service-pattern.md)