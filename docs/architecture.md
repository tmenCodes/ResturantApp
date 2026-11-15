# Architecture

## Tiers

ResturantApp is a 3-tier application. Each tier is deployed and run separately:

| Tier | Technology | Runs on |
|---|---|---|
| Presentation | Angular SPA (`RestaurantFrontend` repo) | Browser |
| Application | ASP.NET Core Web API (this repo) | EC2 |
| Data | PostgreSQL | Database server |

## Vertical slices inside the API

The API is organised by **use case**, not by technical layer. Every endpoint is a **slice**: one folder that holds
everything that endpoint needs (request and response DTOs, a handler, the minimal API endpoint). Changing a
feature means changing one folder.

Slices sit on two shared projects that keep the business rules and the technology out of the slices:

```
            ┌───────────────────────── API ─────────────────────────┐
  HTTP ───▶ │ Features/Cart/AddCartItem/   Features/Account/Login/  │
            │   Endpoint → Handler           Endpoint → Handler     │  ...one folder per endpoint
            │ Common/ (IEndpoint, IHandler, ApiGroups, MVC parity)  │
            └───────────┬───────────────────────────┬───────────────┘
                        ▼                           ▼
            ┌───── Domain ──────┐          ┌──── Infrastructure ─────┐
            │ entities + rules  │ ◀─────── │ EF Core, Identity,      │
            │                   │          │ JWT, queries            │
            └───────────────────┘          └─────────────────────────┘
```

| Project | Namespace root | Responsibility | References |
|---|---|---|---|
| `Domain` | `Resturant.Domain` | Entities and their rules (cart totals, item pricing, checkout creation) | nothing |
| `Infrastructure` | `Resturant.Infrastructure` | EF Core context, configurations, migrations, seed data; Identity account store (`IUserAccountStore`), JWT issuer (`ITokenIssuer`), read-side queries (`IOrderHistoryQueries`), each interface next to its implementation | `Domain`, EF Core, Npgsql, Identity |
| `API` | `API` | `Features/`: the slices. `Common/`: slice plumbing. `Program.cs`: composition root | `Domain`, `Infrastructure` |

### Anatomy of a slice

`API/Features/Cart/AddCartItem/`:

| File | Role |
|---|---|
| `OrderItemDto.cs` | Request body. Property names define the JSON contract |
| `AddToCartResultDto.cs` | Response body |
| `AddCartItemHandler.cs` | The use case: load through `RestaurantContext`, call the entity, `SaveChangesAsync`, map to DTOs. Implements `IHandler` |
| `MenuOrderItemBuilder.cs` | Slice-local helper: looks up the menu item and options and builds the `OrderItem` |
| `AddCartItemEndpoint.cs` | Implements `IEndpoint`: maps the route on an `ApiGroups` group, binds input, calls the handler and turns its result into an HTTP response |

A slice with no logic (for example `Contact/Test`) has only an endpoint. Shapes used by several slices of one feature
live in that feature's `Shared/` folder (`Cart/Shared/CartIdentity`, `Checkout/Shared/CheckoutDto`,
`Account/Shared/FindAccountHandler`, ...).

### Rules

1. **A slice never uses another slice.** It may use `Domain`, `Infrastructure` abstractions, `API.Common` and its own
   feature's `Shared/`. Logic needed by several features goes into `Domain`; logic needed by several slices of one
   feature goes into that feature's `Shared/`.
2. **Endpoints only talk HTTP.** They bind input, call their handler and choose the response. They never take a
   `RestaurantContext`, entity or other Domain/Infrastructure type.
3. **Handlers hold the use case.** There is no service layer: the handler is the use case. A rule about one entity
   is a method on that entity (cart totals live on `ShoppingCart`). Logic a handler needs that is not an entity rule
   stays in the slice, or in the feature's `Shared/` when several slices of that feature need it. Handlers return
   DTOs, never entities.
4. **The Domain Model is plain C#.** It has no EF Core, ASP.NET, JSON or data-annotation attributes. EF maps the
   entities through `IEntityTypeConfiguration<T>` classes in `Infrastructure/Persistence/Configurations`. Entities
   have no public setters and expose collections as read-only; they change only through their own constructors and
   methods, so rules like cart totals can't be bypassed. `Domain.Tests/EncapsulationTests` guards this.
5. **Handlers use `RestaurantContext` directly.** A handler takes the injected `RestaurantContext`, queries it
   (read slices project straight to their DTO) and calls `SaveChangesAsync` once, so related changes are saved
   together. There are no repositories or unit of work.
6. **Reports are queries, not repositories.** Read-only views that join several tables (order history) are query
   interfaces in `Infrastructure/Persistence/Queries` returning read models.
7. **Composition is idempotent.** `AddFeatures()` discovers every `IHandler` (scoped) and `IEndpoint` and registers
   them with `TryAdd*`; `AddInfrastructure(...)` does the same for the outer adapters.
8. **Names are constants.** Route prefixes live in `ApiGroups`; each endpoint declares its own `Route` and message
   constants.
9. **The architecture is tested.** `API.Tests/ArchitectureTests` checks project references (Domain references
   nothing, Infrastructure only Domain).
   `API.Tests/SliceArchitectureTests` checks at IL level that no slice depends on another slice, that every handler
   and endpoint is registered, and that no endpoint, handler result or DTO exposes a Domain or Infrastructure type.

## Behaviour parity with the former MVC controllers

The slices replaced `[ApiController]` controllers without changing what the Angular app receives. Minimal APIs
behave differently by default, so `API/Common` reproduces MVC explicitly:

| MVC behaviour | Slice equivalent |
|---|---|
| `BadRequest("msg")`, `NotFound("msg")`, `Unauthorized("msg")`, `StatusCode(429, "msg")`, string results: `text/plain` | `MvcResults.Message(status, msg)` |
| `NotFound()`, `BadRequest()`, `Unauthorized()` without a body: ProblemDetails with `application/problem+json; charset=utf-8` and a `traceId` | `MvcResults.Problem(status)` → `MvcProblemResult` |
| `Ok(null)` → 204 | `MvcResults.OkOrNoContent(value)` |
| Automatic 400 ValidationProblemDetails for `[Required]` members, non-nullable members, missing non-nullable query parameters and empty bodies | `MvcValidationFilter`, applied by every `ApiGroups` group. Endpoints declare inputs nullable and opt in with `[Required]` on parameters and DTO members and `[FromBody]` on bodies. **Parameter names are error keys**, so they match the former action parameters (`orderItemDto`, `userDTO`, ...) |
| Missing `int`/`Guid` query values bound to their defaults | Nullable parameters with `?? default` (`RemoveCartItemEndpoint`) |
| `DomainExceptionFilter`: `NotFoundException` → 404 `text/plain` | `DomainExceptionEndpointFilter`, applied by every group |
| `AddJsonOptions(ReferenceHandler.IgnoreCycles)` | `ConfigureHttpJsonOptions` in `AddFeatures()` |
| `[Authorize]` | `.RequireAuthorization()` |

`API.Tests/Integration` holds the **HTTP contract tests**: they run the real API in memory and pin every route's status,
content type and body. They were written against the controllers before the migration and kept passing unchanged,
which shows the frontend contract held. Any change to an endpoint must keep them green.

### Known differences

Malformed (not missing) input keeps MVC's status code but the body is now empty instead of ValidationProblemDetails:
an unparsable query value (`DeleteMenuItem?id=abc`), invalid JSON (400), and a non-JSON `Content-Type` (415). The
Angular app never sends these. Rebuilding MVC's bodies would need status-code-page middleware, which would also
change the 401 and unknown-route 404 responses that currently match MVC exactly.

## Project layout

```
Domain/
  Menu/, Cart/, Ordering/, Customers/, ContactInfo/   entities
  Payments/                                  CreditCardInformation: card details held by CheckoutInformation and
                                             SavedPaymentMethod, stored in their own columns as an EF owned type
Infrastructure/
  Persistence/          RestaurantContext, Configurations/, Queries/, Migrations/
  Identity/             WebUser, IUserAccountStore, IdentityUserAccountStore, UserAccount, ...
  Auth/                 JwtSettings, ITokenIssuer, JwtTokenIssuer
  InfrastructureServiceCollectionExtensions.cs   AddInfrastructure()
API/
  Common/               IEndpoint, IHandler, FeatureRegistration (AddFeatures, MapFeatureEndpoints), ApiGroups,
                        MvcResults, MvcProblemResult, MvcValidationFilter, DomainExceptionEndpointFilter
  Features/<Feature>/<Slice>/                    one folder per endpoint; <Feature>/Shared/ for shared shapes
  Program.cs
Domain.Tests/           entity rules and encapsulation
API.Tests/              Integration/ contract tests, Features/ handler tests, Infrastructure and architecture tests
```

## Slice catalog

| Route (unchanged) | Slice |
|---|---|
| `GET api/Order/GetMenuItems`, `GetMenuItemOptions`, `ImagePath` | `Menu/GetMenuItems`, `Menu/GetMenuItemOptions`, `Menu/GetImagePaths` |
| `POST api/Order/ShoppingCartItems` | `Cart/AddCartItem` |
| `DELETE api/Order/DeleteMenuItem` | `Cart/RemoveCartItem` |
| `GET api/Order/GetTotalPrice`, `GetMenuItemByGuid`, `GetMenuItemByGuidSize`, `GetAllTempItems` | `Cart/GetCartTotals`, `Cart/GetCartItems`, `Cart/GetCartItemCount`, `Cart/GetAllOrderItems` |
| `POST api/Payment/PaymentInformation` | `Checkout/CheckoutCart` |
| `GET api/Payment/GetAllOrders`, `GetAllOrdersWithMenu` | `Checkout/GetAllCheckouts`, `Checkout/GetAllOrdersWithMenu` |
| `POST api/RegisterUser/Login`, `SignOut`, `CreateUser` | `Account/Login`, `Account/Logout`, `Account/CreateUser` |
| `GET api/RegisterUser/CheckDuplicateEmail`, `FindByUser` | `Account/CheckDuplicateEmail`, `Account/FindByUser` |
| `GET api/RegisterUser/ReturnAllUsers`, `GetTheCurrentUser`, `IsUserLoggedIn`, `user` (signed in) | `Account/GetAllUsers`, `Account/GetCurrentUser`, `Account/IsUserLoggedIn`, `Account/AuthCheck` |
| `POST/GET/PUT api/UserInformation/...PaymentInformation` (signed in) | `UserProfile/AddPaymentMethod`, `GetPaymentMethod`, `UpdatePaymentMethod` |
| `GET api/UserInformation/UserHistoryPayments`, `UserHistoryMenu` (signed in) | `UserProfile/GetRecentOrderTotals`, `UserProfile/GetOrderedItems` |
| `POST api/Misc/BookingInformation`, `CustomerInquiry` | `Contact/CreateReservation`, `Contact/CreateInquiry` |
| `GET api/Misc/CreateGuideForFrontend`, `Test` | `Contact/CreateGuid`, `Contact/Test` |

## Unchanged contracts

- Every route, HTTP method and JSON request/response shape stays the same.
- The database schema stays the same. `MigrationsAssembly` is `Resturant.Infrastructure`, and migration IDs in
  `__EFMigrationsHistory` don't depend on the assembly, so nothing needs re-applying.

## Known issues

- Card numbers and CVVs are stored and returned in plain text (`CheckoutInformation`, `SavedPaymentMethod`). That's a PCI-DSS
  concern to handle separately.
- `UserHistoryPayments` sorts dates as text, so "10/1" comes before "9/1".
