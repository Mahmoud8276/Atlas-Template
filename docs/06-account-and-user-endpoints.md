# 06 — Account & User Endpoints

Atlas ships two fully implemented controllers — `AccountController` and `UsersController` — that together demonstrate the whole template working end to end: Identity, JWT, validation, file uploads, email, and the response envelope, all in real, runnable code you can read alongside the other docs.

## `AccountController` — self-service endpoints

Everything here acts on *the calling user* — there's no admin override and no id parameter for most actions, since the user is identified entirely by their JWT (via `GetRequiredUserId()`, see [`docs/05`](05-authentication-and-jwt.md)).

| Endpoint | What it does |
|---|---|
| `POST /register` | Creates the `AppUser`, assigns the default `User` role, sends a confirmation email. Registration and role assignment run inside a transaction — if either fails, the user (and any uploaded image) is rolled back |
| `POST /login` | Verifies credentials, reuses an existing active refresh token if one exists, returns an access token + sets the refresh token as an HttpOnly cookie |
| `GET` / `POST /confirm-email` | The `GET` verifies a token from an email link; the `POST` re-sends the confirmation email (used when the original didn't arrive) |
| `POST /forget-password` | Sends a reset link. Always responds with the same success message whether or not the email exists — see below for why |
| `POST /reset-password` | Verifies the reset token and sets a new password |
| `GET /refresh-token` | Reads the refresh token from the cookie, rotates it, returns a new access token |
| `POST /revoke-token` | Explicit logout — revokes a refresh token so it can't be used again |
| `GET /me` *(auth required)* | Returns the current user's profile |
| `PATCH /me` *(auth required)* | Updates profile fields and/or the profile image; only non-null fields in the DTO are applied |
| `DELETE /me` *(auth required)* | Deletes the account and its uploaded image |
| `POST /me/change-password` *(auth required)* | Changes password given the current one |

### Why `forget-password` always says "check your email"

```csharp
var user = await _userManager.FindByEmailAsync(dto.Email);
if (user == null)
    return Response.Success(message: "Check your email");
```

If this endpoint returned a different message for "email not found" vs. "email sent", it would let anyone probe which email addresses have an account — a user enumeration vulnerability. Responding identically either way closes that off. The same reasoning is why `send-confirm-email` returns success even if the user's email is already confirmed.


## `UsersController` — admin endpoints

```csharp
[Authorize(Roles = "Admin")]
public class UsersController : BaseController
```

The whole controller requires the `Admin` role — there's no per-action override, so every endpoint here is admin-only by default.

| Endpoint | What it does |
|---|---|
| `GET /users` | Paginated, filterable list of all users |
| `GET /users/{id}` | A single user's details by id |

`UserService.GetAllAsync` takes a `UserSpecParams` (just `UserName`, plus the inherited `PageIndex`/`PageSize`) and filters by first+last name containing the search string, case-insensitively. As noted in [`docs/03-specifications.md`](03-specifications.md), this doesn't go through the specification pattern — it queries `UserManager.Users` directly with LINQ, since `AppUser` is Identity-managed rather than repository-managed. If you add a `ProductsController` or similar on top of the generic repository, you'd use `ISpecification` there instead, the way [`docs/03`](03-specifications.md) and [`docs/04`](04-read-write-service-pattern.md) demonstrate.

---

**Next:** [07 — Validation](07-validation.md)