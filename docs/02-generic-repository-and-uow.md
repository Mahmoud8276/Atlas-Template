# 02 — Generic Repository & Unit of Work

Most entities need the same handful of data-access operations — get by id, get all, add, update, delete. Writing a dedicated repository class for every entity means repeating that same code over and over. The generic repository exists so you get all of that for free, and only write custom data-access code for the cases that are actually special.

## `IGenericRepository<TModel, TKey>`

Defined in Core, implemented in Infrastructure as `GenericRepository<TModel, TKey>`. Any entity that inherits `BaseModel<TKey>` can use it as-is, with no extra code.

- **`GetByIdAsync(id)`** — single entity by primary key
- **`FindAsync(condition)`** — entities matching a LINQ expression, for simple ad-hoc filters
- **`GetAllAsync()`** — every row (use carefully — no paging)
- **`AddAsync(model)` / `AddRangeAsync(models)`** — stage one or many new entities
- **`UpdateAsync(model)` / `DeleteAsync(model)` / `DeleteRangeAsync(models)`** — stage changes or removals
- **`GetWithSpecAsync(spec)` / `GetAllWithSpecAsync(spec)` / `GetCountWithSpecAsync(spec)`** — run a specification (filtering, ordering, includes, pagination) through `SpecificationEvaluator`. Covered in depth in [`docs/03-specifications.md`](03-specifications.md)

**Important:** `AddAsync`, `UpdateAsync`, and `DeleteAsync` only *stage* the change on the `DbContext` — nothing is written to the database yet. That's intentional, and it's the whole reason `IUnitOfWork` exists separately.

## `IUnitOfWork`

- **`CompleteAsync()`** — calls `SaveChangesAsync()`, committing every staged change since the last save in a single round trip
- **`BeginTransactionAsync()` / `CommitTransactionAsync()` / `RollbackTransactionAsync()`** — for the cases where several `CompleteAsync()` calls need to succeed or fail together

**Why staging and committing are separate steps:** it lets a service perform several repository operations — maybe against more than one entity type — and persist them all in one `SaveChangesAsync()` call. If you called `AddAsync` and it hit the database immediately, you'd lose the ability to treat a multi-step operation as one atomic unit.

## Two ways to get a repository for a new entity

You have two options, and the right one depends on whether the entity needs anything beyond standard CRUD.

### Option A — use the generic repository directly

If an entity doesn't need any custom queries, don't build anything extra — just register the generic repository for it and inject `IGenericRepository<TModel, TKey>` straight into your service:

```csharp
// Repositories.cs
services.AddScoped<IGenericRepository<Product, int>, GenericRepository<Product, int>>();
```

```csharp
public class ProductService : WriteService<Product, ProductDetailsDto, ProductCreateDto, ProductUpdateDto, int>
{
    public ProductService(IGenericRepository<Product, int> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork) { }
}
```

### Option B — a dedicated repository for entity-specific queries

If `Product` needs a query the generic repository can't express — something like "get all products below a stock threshold" — extend the generic repository instead of bolting extra methods onto the generic interface. Expose it through `IUnitOfWork` as a named, read-only property so it's discoverable alongside everything else the unit of work coordinates.

**1. Define the contract**, inheriting the generic one so you keep all the standard operations for free:

```csharp
public interface IProductRepository : IGenericRepository<Product, int>
{
    Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold);
}
```

**2. Implement it**, inheriting `GenericRepository<Product, int>` so only the new method needs real code:

```csharp
public class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold)
    {
        return await _context.Set<Product>()
            .Where(p => p.StockQuantity < threshold)
            .ToListAsync();
    }
}
```

**3. Expose it on `IUnitOfWork`** as a read-only property:

```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    IProductRepository ProductRepository { get; }

    Task CompleteAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

**4. Wire it up in `UnitOfWork`**, using `Lazy<T>` so the repository is only constructed the first time something actually asks for it:

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Lazy<IProductRepository> _productRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(_context));
    }

    public IProductRepository ProductRepository => _productRepository.Value;

    // CompleteAsync, transaction methods, etc.
}
```

**5. Use it normally from a service** — no separate DI registration needed, since it's reached through the already-registered `IUnitOfWork`:

```csharp
var lowStock = await _unitOfWork.ProductRepository.GetLowStockAsync(10);
```

Mixing both options in the same project is fine — most entities are happy with Option A, and you reach for Option B only when an entity's queries genuinely don't fit the generic shape.

## Worked example: adding and saving a `Product`

```csharp
public async Task<Response<ProductDetailsDto>> CreateAsync(ProductCreateDto dto)
{
    var product = dto.Adapt<Product>();

    await _repository.AddAsync(product);   // staged, not saved yet
    await _unitOfWork.CompleteAsync();     // now it's written to the database

    return Response<ProductDetailsDto>.Success(product.Adapt<ProductDetailsDto>());
}
```

If you're building this on top of `WriteService` rather than by hand, this whole method is already written for you — see [`docs/04-read-write-service-pattern.md`](04-read-write-service-pattern.md).

---

**Next:** [03 — Specifications](03-specifications.md)