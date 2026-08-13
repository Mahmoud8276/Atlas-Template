# 10 — API Versioning & Responses

Two conventions run through every endpoint in Atlas: every route is versioned, and every response — success or failure — comes back in the same envelope shape. Neither is enforced by the compiler, but both are cheap to keep once you know where they're wired in.

## API versioning

Atlas versions by **URL segment** (`/api/v1/...`, `/api/v2/...`) rather than a header or query string. It's the most visible option — anyone reading a URL, a log line, or a Swagger doc immediately knows which version they're looking at, at the cost of the version being baked into the route itself rather than negotiated separately.

**`BaseController`** puts the version placeholder in the route template once, so every controller inherits it automatically:

```csharp
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class BaseController : ControllerBase { }
```

**Each controller declares which version(s) it belongs to:**

```csharp
[ApiVersion("1.0")]
public class AccountController : BaseController
```

**`Program.cs` wires up the versioning services:**

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddMvc();

builder.Services.AddApiVersioning()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
```

- `AssumeDefaultVersionWhenUnspecified` means a request to a route without an explicit version doesn't just fail — it's treated as v1.
- `ReportApiVersions` adds an `api-supported-versions` response header, so a client can discover what's available without checking docs.
- `AddApiExplorer`'s `GroupNameFormat`/`SubstituteApiVersionInUrl` are what let Swagger generate a correctly separated document per version (`swagger/v1/swagger.json`, `swagger/v2/swagger.json`), matching the `SwaggerDoc("v1", ...)`/`SwaggerDoc("v2", ...)` registrations right above it.

**Adding a `v2` endpoint**, once you actually need one, means a new controller in a `V2` folder/namespace with `[ApiVersion("2.0")]`, sharing the same route template from `BaseController`. Existing v1 clients are unaffected since their URLs still resolve to the v1 controller.

## The response envelope

Every service method returns a `Response` (or, if you choose to use it, `Response<T>`) rather than the raw result or a thrown exception for expected outcomes — see [`docs/08-exception-handling.md`](07-exception-handling.md) for when a thrown `AppException` is the better fit instead.

```csharp
public class BaseResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
    public int StatusCode { get; set; }
}

public class Response : BaseResponse
{
    public object Data { get; set; }
    public static Response Fail(string message = "Fail!", int statusCode = 400, string details = null) => ...
    public static Response Success(object data = null, string message = "Success!", int statusCode = 200) => ...
}
```

There's also a generic `Response<T>` with a strongly-typed `Data`, and `Api.Responses.ApiResponse`/`ApiResponse<T>` (empty subclasses, used in a couple of places in `AccountController` — e.g. `ApiResponse.Fail("Refresh token is required.")`). `ReadService`/`WriteService` consistently use the non-generic `Response`, so `Data` is boxed as `object`; nothing stops a hand-written service from using `Response<T>` instead if you want the extra compile-time safety.

**Why every response — success or failure — uses the same shape:** a client can always check `IsSuccess` and read `Message`/`Data` the same way, regardless of which endpoint it called or whether that call succeeded. There's no separate "error response" DTO to special-case.

**How a controller uses it** — this is the one line every action in the template ends with:

```csharp
var result = await _productService.CreateAsync(dto);
return StatusCode((int)result.StatusCode, result);
```

The controller doesn't decide the HTTP status code — the service already put the right one on `result.StatusCode` (`200`/`201` for success, `400`/`404`/`409` for the various failure cases), and the controller just relays it. This is also why controllers stay thin — there's no `if (result.IsSuccess) return Ok(...) else return BadRequest(...)` branching to write, because `StatusCode(...)` handles every case uniformly.

---

**Next:** [11 — Configuration & Options](11-configuration-and-options.md)