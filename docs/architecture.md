# Architecture

> **Status: proposal.** This describes the target design. The code moves to it in the phases listed at the end.

## Tiers

ResturantApp is a classic **3-tier** application. Each tier is deployed and run separately:

| Tier | Technology | Runs on |
|---|---|---|
| Presentation | Angular SPA (`RestaurantFrontend` repo) | Browser |
| Application | ASP.NET Core Web API (this repo) | EC2 |
| Data | PostgreSQL | Database server |

## Layers inside the API

The API tier is a classic **3-layer** design. Dependencies point one way only:

```
API (Presentation)  ──►  Business  ──►  Data  ──►  PostgreSQL
```

| Project | Namespace root | Responsibility | May reference |
|---|---|---|---|
| `API` | `API` | HTTP only: routing, model binding, auth attributes, status codes, `Program.cs` composition | `Business` |
| `Business` | `Resturant.Business` | Business rules and use cases: cart pricing, checkout, user history | `Data` |
| `Data` | `Resturant.Data` | Entities, `RestaurantContext`, migrations, seed data, repositories, ASP.NET Identity store (`WebUser`) | EF Core, Npgsql, Identity |

### Rules

1. **Controllers never touch `RestaurantContext`, repositories, `UserManager` or `SignInManager`.** They call a Business service and translate its result into an HTTP response.
2. **Business never references ASP.NET Core MVC types** (`IActionResult`, `HttpContext`, `[FromBody]`). Services return DTOs, or simple result types that the controller maps to status codes.
3. **Entities never leave the Data and Business layers.** Services take and return DTOs from `Business/Dtos`. A DTO that replaces an entity in a response keeps the same property names, so **the JSON the Angular app receives does not change**.
4. **Data contains no business rules.** Repositories run queries and persist changes, and that's all.
5. **The API can't reach Data at compile time.** `Business` references `Data` with `PrivateAssets="all"`, so Data types don't flow through to `API`. Data registers itself through `AddDataLayer(...)`, called from Business's `AddBusinessLayer(...)`, which `Program.cs` calls.
6. **Dependencies are injected through interfaces.** Services depend on repository interfaces (defined in Data), and controllers depend on service interfaces (defined in Business).

## Project layout

```
API/
  Controllers/          thin controllers, one per resource
  Contracts/            HTTP-only request shapes (e.g. LoginDTO)
  Program.cs            composition root: auth/JWT, CORS, Swagger, AddBusinessLayer()
Business/
  Dtos/                 service inputs/outputs (from Application/DTO + entity-shaped responses)
  Services/             I<Name>Service + <Name>Service
  BusinessServiceCollectionExtensions.cs   AddBusinessLayer()
Data/
  Entities/             MenuItem, ShoppingCart, Checkout, ... (from Domain/Entity), Event (Events table)
  Identity/WebUser.cs
  Context/RestaurantContext.cs
  Repositories/         I<Name>Repository + <Name>Repository
  SeedData/
  Migrations/           (gitignored today; path in .gitignore updated)
  DataServiceCollectionExtensions.cs       AddDataLayer()
Business.Tests/         xUnit + EF Core InMemory
```

## Service and repository catalog

| Controller (routes unchanged) | Business service | Data repositories |
|---|---|---|
| `OrderController`, menu endpoints | `MenuService` | `IMenuRepository` |
| `OrderController`, cart endpoints | `CartService` (pricing and option totals from `AddMenuItems`) | `IShoppingCartRepository`, `IMenuRepository` |
| `PaymentController` | `CheckoutService` | `ICheckoutRepository`, `IShoppingCartRepository` |
| `UserInformationController` | `UserProfileService` (saved payment methods, order history) | `ISavedPaymentMethodRepository`, `ICheckoutRepository` |
| `RegisterUser` | `AccountService` (create user, login, JWT, lookups) | ASP.NET Identity `UserManager<WebUser>` / `SignInManager<WebUser>` |
| `MiscController` | `ContactService` (reservations, inquiries) | `IContactRepository` |

## What is removed

These are Domain-Driven Design / MediatR pieces with no active use: `AggregateRoot`, the `Entity` base class, `DomainEvents/*`, `PriceUpdatedHandlerDomainEvent`, `PostCartItems`/`TempDto`, the MediatR registration and package, and the unregistered `DatabaseBackGround` service. `EventSourcing/Event` is **kept** (moved to `Data/Entities`) because it maps to the `Events` table.

## Unchanged contracts

- Every route, HTTP method and JSON request/response shape stays the same.
- The database schema stays the same. Entity namespaces change (`Resturant.Domain.Entity` → `Resturant.Data.Entities`), so the type names in the migration snapshot are updated to match, and EF must report no pending model changes. `MigrationsAssembly` changes from `Resturant.Infrastructure` to `Resturant.Data`. Migration IDs in `__EFMigrationsHistory` don't depend on the assembly, so nothing needs re-applying.

## Migration phases

Each phase is its own commit and must build, pass the existing tests and pass a smoke test of the touched endpoints.

1. **Structure.** Merge `Domain` + `Infrastructure` into `Data`, rename `Application` to `Business`, reverse the dependency direction, delete the dead code, and update the solution, test references, README and `dotnet ef` commands. No behavior change.
2. **`MiscController`** → `ContactService`
3. **`PaymentController`** → `CheckoutService`
4. **`OrderController`** → `MenuService` + `CartService`
5. **`UserInformationController`** → `UserProfileService`
6. **`RegisterUser`** → `AccountService`
7. **Enforce the boundary.** Add `PrivateAssets="all"` and composition through `AddBusinessLayer()`. The API no longer compiles against Data.

Each service phase adds `Business.Tests` coverage for that service, using xUnit and EF Core InMemory.

## Known issues (not fixed by this refactor)

- `GET api/Payment/GetAllOrdersWithMenu` runs `ShoppingCarts.Include("MenuItems")`, but that navigation doesn't exist, so the endpoint throws. The query's result is unused (the endpoint returns a fixed string), so phase 3 drops it rather than carrying the bug into `CheckoutService`.
- Card numbers and CVVs are stored and returned in plain text (`Checkout`, `SavedPaymentMethod`). That's a PCI-DSS concern to handle separately.
