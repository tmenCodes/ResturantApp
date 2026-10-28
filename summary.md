# 3-Tier Architecture Refactor: Summary

## Goal

Finish moving the API onto a strict 3-tier design:

```
API (controllers, request contracts)  →  Business (services, DTOs, mappings)  →  Data (EF Core context, entities, repositories)
```

Controllers depend only on Business. Only `Program.cs`, the composition root, references Data.

## Starting point

Contact, Checkout, Menu and Cart had already been moved behind services and repositories. An audit found what was still left:

| Area | Problem |
|---|---|
| `UserInformationController` | Injected `RestaurantContext` and ran LINQ joins in the controller. It also loaded whole tables into memory |
| `RegisterUser` controller | Did user creation, username de-duplication and JWT generation inline, using `UserManager`/`SignInManager` |
| `API/CheckingPrices.cs` | Hard-coded menu prices in the API layer, with nothing referencing them |
| `Business/Dtos/OrderItemDto.cs` | Had an unused import of a Data entity |

## Changes

| Commit | Change |
|---|---|
| `bdd127b` refactor(user) | Added `IUserProfileRepository`/`UserProfileRepository` (the history joins now run in SQL) and `IUserProfileService`/`UserProfileService`. `UserInformationController` now depends only on the service |
| `0949cc5` refactor(auth) | Added `IAccountService`/`AccountService` and `IJwtTokenGenerator`/`JwtTokenGenerator`, with settings bound from `IOptions<JwtSettings>`. `RegisterUser` now depends only on the service |
| `354ddf0` refactor(api) | The DbContext and repositories are now registered through `AddDataLayer(...)`. Deleted `CheckingPrices.cs` and the unused import |
| `decf958` fix(auth) | Added `[Authorize]` to `ReturnAllUsers` and `UpdateUserPaymentInformation` |
| `dcbad3a` ci | Commented out the EC2 deploy workflow (`aws.yml`) so pushes to `main` don't deploy |

## Behavior changes

- `GetTheCurrentUser` and `ReturnAllUsers` now return `AccountUserDto` (`id`, `userName`, `email`, `fullName`). Before, they returned the raw Identity user, including `passwordHash` and `securityStamp`.
- `ReturnAllUsers` and `UpdateUserPaymentInformation` now return 401 when no token is sent.
- The payment-info endpoints read the username from the JWT instead of looking the user up in the database.
- All other routes, status codes and response shapes are the same.

## Verification

- `dotnet build`: 0 errors. Warnings went from 26 to 13, all from older code.
- `dotnet test Business.Tests`: 40 tests pass, up from 22. The new ones are `UserProfileServiceTests`, `AccountServiceTests` (real Identity on an in-memory store) and `JwtTokenGeneratorTests`.
- Smoke test against the local Postgres: Swagger, the login error cases, the duplicate email/username checks and the 401 responses on protected endpoints all behaved as expected.
- Not yet tested end to end: a successful login, and the logged-in history and payment endpoints.

## Open items

- **PCI DSS:** `SavedPaymentMethod` and `Checkout` store the full card number and CVV. Store only a tokenized reference from a payment provider instead.
- **Roles:** any logged-in user can call `ReturnAllUsers`. Restrict it to admins or remove it.
- **Date sorting:** `UserHistoryPayments` sorts dates as text, as it did before, so "10/1" comes before "9/1".
- **Frontend:** check that the Angular app works with the smaller user responses and the new 401s.
- **Deploy:** uncomment `.github/workflows/aws.yml` to turn EC2 deploys back on.
- **Repository:** the GitHub repo moved to `tmenCodes/ResturantApp`. Update the remote with `git remote set-url origin https://github.com/tmenCodes/ResturantApp.git`.
- **Old tests:** the `OrderControllerTest`, `UnitTesting1` and `TestProject1` folders are no longer in the solution and can be deleted.
