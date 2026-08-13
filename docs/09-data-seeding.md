# 9 — Data Seeding

New environments need baseline data before the API is actually usable — at minimum, the roles your `[Authorize(Roles = "...")]` attributes refer to have to exist, or every role check fails regardless of how correct your code is. Data seeders exist to guarantee that baseline data is always in place, automatically, every time the app starts.

## `IDataSeeder`

```csharp
public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync(CancellationToken token = default);
}
```

Deliberately minimal — just an order and a method. Anything more specific (how to check if data already exists, what to insert) is entirely up to each seeder's own implementation.

## How seeders are discovered and run

Nothing manually lists which seeders exist — `AddDataSeeders` (Services layer) finds them by reflection:

```csharp
var seederTypes = infrastructureAssembly
    .GetTypes()
    .Where(t => typeof(IDataSeeder).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

foreach (var type in seederTypes)
    serviceCollection.AddScoped(typeof(IDataSeeder), type);
```

**Why auto-discovery instead of a manual list:** if adding a new seeder meant remembering to also register it somewhere, it's easy to write the class, forget the registration line, and end up debugging why your data never appears. Scanning the assembly means the only thing you ever have to do is create the class — implementing `IDataSeeder` *is* the registration.

At startup, `DataSeedersRunner.RunAsync()` (called from `SeedData.SeedDataAsync`, which runs right after migrations in `Program.cs`) resolves every registered `IDataSeeder` and runs them **in `Order` order**:

```csharp
foreach (var seeder in _seeders.OrderBy(s => s.Order))
    await seeder.SeedAsync(cancellationToken);
```

## The two seeders that ship with the template

- **`RoleDataSeeder`** (`Order => 1`) — creates a role for every value in the `AppUserRoles` enum that doesn't already exist. Runs first, deliberately, because...
- **`AdminDataSeeder`** (`Order => 2`) — creates a small hardcoded list of admin accounts, skipping any whose email already exists, and assigns each the `Admin` role. This has to run *after* `RoleDataSeeder`, since assigning the `Admin` role only works if that role already exists — that dependency is exactly what `Order` is for.

**Worth knowing before you deploy this anywhere real:** `AdminDataSeeder` currently has real-looking names, an email address, and a plaintext password hardcoded directly in the source file. Treat that list as placeholder data to replace, not as something to ship — swap in your own accounts (or better, read them from configuration/secrets rather than hardcoding them at all) before this runs against a real database.

## Every seeder should be idempotent

Both existing seeders check for existing data before inserting (`Except(existingRoles)`, `FindByEmailAsync(...) != null → skip`). This matters because seeders run on **every application startup**, not just the first one — without that check, `AdminDataSeeder` would try to create the same admin accounts every time the app restarts and fail on the duplicate email.

## Worked example: seeding default `Category` rows

Say new `Product`s need to belong to a `Category`, and you want a few default categories to exist in every environment:

```csharp
public class CategoryDataSeeder : IDataSeeder
{
    public int Order => 3; // after roles/admin, but order doesn't matter much here since it's unrelated data

    private static readonly string[] _defaultCategories = { "Electronics", "Books", "Clothing" };

    private readonly AppDbContext _context;
    public CategoryDataSeeder(AppDbContext context) => _context = context;

    public async Task SeedAsync(CancellationToken token = default)
    {
        var existing = await _context.Set<Category>()
            .Select(c => c.Name)
            .ToListAsync(token);

        var missing = _defaultCategories.Except(existing);

        foreach (var name in missing)
            await _context.Set<Category>().AddAsync(new Category { Name = name }, token);

        await _context.SaveChangesAsync(token);
    }
}
```

Because this lives in `Atlas.Template.Infrastructure` (the same assembly `AddDataSeeders` scans) and implements `IDataSeeder`, nothing else needs to change — no DI registration, no manual wiring. It's picked up and run automatically the next time the app starts.

---

**Next:** [10 — API Versioning & Responses](10-api-versioning-and-responses.md)