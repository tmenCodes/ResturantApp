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
| `Infrastructure` | `Resturant.Infrastructure` | EF Core context, configurations, migrations, seed data; Identity account store (`IUserAccountStore`), JWT issuer (`ITokenIssuer`), each interface next to its implementation | `Domain`, EF Core, Npgsql, Identity |
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

A slice with no logic (for example `Account/Logout`) has only an endpoint. Shapes used by several slices of one feature
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
   (read slices project straight to their DTO, reports that join several tables included) and calls
   `SaveChangesAsync` once, so related changes are saved together. There are no repositories, unit of work or
   query interfaces.
6. **Composition is idempotent.** `AddFeatures()` discovers every `IHandler` (scoped) and `IEndpoint` and registers
   them with `TryAdd*`; `AddInfrastructure(...)` does the same for the outer adapters.
7. **Names are constants.** Route prefixes live in `ApiGroups`; each endpoint declares its own `Route` and message
   constants.
8. **The architecture is tested.** `API.Tests/ArchitectureTests` checks project references (Domain references
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
  Payments/                                  CreditCardInformation: card details held by Order and
                                             SavedPaymentMethod, stored in their own columns as an EF owned type
Infrastructure/
  Persistence/          RestaurantContext, Configurations/, Migrations/
  Identity/             WebUser, IUserAccountStore, IdentityUserAccountStore, UserAccount, ...
  Auth/                 JwtSettings, ITokenIssuer, JwtTokenIssuer
  InfrastructureServiceCollectionExtensions.cs   AddInfrastructure()
API/
  Common/               IEndpoint, IHandler, FeatureRegistration (AddFeatures, MapFeatureEndpoints), ApiGroups,
                        MvcResults, MvcProblemResult, MvcValidationFilter, DomainExceptionEndpointFilter,
                        AuthorizationPolicies
  Features/<Feature>/<Slice>/                    one folder per endpoint; <Feature>/Shared/ for shared shapes
  Program.cs
Domain.Tests/           entity rules and encapsulation
API.Tests/              Integration/ contract tests, Features/ handler tests, Infrastructure and architecture tests
```

## Slice catalog

Each route group matches one business area. Auth: none unless noted.

| Route | Slice |
|---|---|
| `GET api/Menu/GetMenuItems`, `GetMenuItemOptions` | `Menu/GetMenuItems`, `Menu/GetMenuItemOptions` |
| `GET api/Cart/CreateCart` | `Cart/CreateGuid` |
| `POST api/Cart/AddItem` | `Cart/AddCartItem` |
| `GET api/Cart/GetItems`, `GetItemCount`, `GetTotals` | `Cart/GetCartItems`, `Cart/GetCartItemCount`, `Cart/GetCartTotals` |
| `DELETE api/Cart/RemoveItem` | `Cart/RemoveCartItem` |
| `POST api/Cart/Checkout` (token optional; see Authorization) | `Checkout/CheckoutCart` |
| `POST api/Registration/CreateUser`; `GET api/Registration/CheckDuplicateEmail`, `FindByUser` | `Account/CreateUser`, `Account/CheckDuplicateEmail`, `Account/FindByUser` |
| `POST api/Login/Login`, `SignOut`; `GET api/Login/IsUserLoggedIn` (signed in) | `Account/Login`, `Account/Logout`, `Account/IsUserLoggedIn` |
| `POST api/UserInformation/AddPaymentMethod`; `GET GetPaymentMethod`, `UserHistoryPayments`; `PUT UpdatePaymentMethod` (signed in) | `UserProfile/AddPaymentMethod`, `GetPaymentMethod`, `GetRecentOrderTotals`, `UpdatePaymentMethod` |
| `POST api/Contact/MakeReservation`, `SendInquiry` | `Contact/CreateReservation`, `Contact/CreateInquiry` |
| `GET api/Admin/ReturnAllUsers`, `GetAllOrders` (SuperAdmin) | `Account/GetAllUsers`, `Checkout/GetAllCheckouts` |

## Authorization

- `.RequireAuthorization()` means any valid JWT. `.RequireAuthorization(AuthorizationPolicies.SuperAdmin)` also
  requires the `SeedSuperAdmin.Role` role. The `api/Admin` group itself requires `AuthorizationPolicies.SuperAdmin`
  (in `ApiGroups.Admin`), so every endpoint mapped there (`ReturnAllUsers`, `GetAllOrders`) is super admin only
  without repeating the policy.
- `POST api/Cart/Checkout` allows guests. It saves the order for the token's user (`ClaimTypes.Name`) when a valid
  JWT is sent, and for nobody otherwise. The buyer is never read from the request body, so a client can't put an
  order into someone else's `UserHistoryPayments`.
- Policies are HTTP pipeline concerns, so `AuthorizationPolicies` lives in `API/Common`; it builds on the role name
  that Infrastructure owns (`SeedSuperAdmin.Role`). Endpoints import the policy from `API.Common`, not Infrastructure.
- Roles are ASP.NET Core Identity roles stored in the database (`AddRoles<IdentityRole>()`). `LoginHandler` reads the
  account's roles through `IUserAccountStore.GetRolesAsync` and `JwtTokenIssuer` writes one role claim per role, so a
  role change takes effect at the next login.
- `Program.cs` calls `app.Services.SeedSuperAdminAsync()` after the app is built.
  `Infrastructure/Identity/SeedSuperAdmin.cs` creates the `SuperAdmin` role, the `admin@gmail.com` account (with the
  plain-text password in that file, for local deployment only) and the link between them, each only if missing.
  It never resets an existing password. Startup fails if Identity rejects the role, account or password.
- `ApiFactory` creates the seeded in-memory database before the app starts, so contract tests log in as
  `ApiFactory.AdminEmail` with `SeedSuperAdmin.Password`.

## Route changes

The routes were regrouped by business area. Every old URL below returns 404, so the API and the Angular frontend
(`http.service.ts`, `cart.service.ts`) must be deployed together. JSON request and response shapes are unchanged,
except where noted.

| Old | New |
|---|---|
| `api/Order/GetMenuItems`, `GetMenuItemOptions` | `api/Menu/...` (same names) |
| `GET api/Misc/CreateGuideForFrontend` | `GET api/Cart/CreateCart` |
| `POST api/Order/ShoppingCartItems` | `POST api/Cart/AddItem` |
| `GET api/Order/GetMenuItemByGuid` | `GET api/Cart/GetItems` |
| `GET api/Order/GetMenuItemByGuidSize` | `GET api/Cart/GetItemCount` |
| `GET api/Order/GetTotalPrice` | `GET api/Cart/GetTotals` |
| `DELETE api/Order/DeleteMenuItem` | `DELETE api/Cart/RemoveItem` |
| `POST api/Payment/PaymentInformation` | `POST api/Cart/Checkout`; the body's `userName` is now ignored |
| `POST api/RegisterUser/CreateUser`; `GET CheckDuplicateEmail`, `FindByUser` | `api/Registration/...` (same names) |
| `POST api/RegisterUser/Login`, `SignOut`; `GET IsUserLoggedIn` | `api/Login/...` (same names) |
| `GET api/RegisterUser/ReturnAllUsers` (any signed-in user) | `GET api/Admin/ReturnAllUsers` (SuperAdmin) |
| `GET api/Payment/GetAllOrders` (public) | `GET api/Admin/GetAllOrders` (SuperAdmin) |
| `api/UserInformation/AddUserPaymentInformation`, `GetUserPaymentInformation`, `UpdateUserPaymentInformation` | `api/UserInformation/AddPaymentMethod`, `GetPaymentMethod`, `UpdatePaymentMethod` |
| `POST api/Misc/BookingInformation` | `POST api/Contact/MakeReservation` |
| `POST api/Misc/CustomerInquiry` | `POST api/Contact/SendInquiry` |

Removed, because nothing called them:
- `GET api/RegisterUser/user`, which only confirmed a token. `IsUserLoggedIn` covers that.
- `GET api/Order/GetAllTempItems`, which listed every cart's items to anyone.
- `GET api/Payment/GetAllOrdersWithMenu`, a placeholder that returned "completed for now".
- `GET api/UserInformation/UserHistoryMenu`.
- `GET api/Order/ImagePath`, which repeated the image names that `GetMenuItems` already returns in
  `menuItemImageUrl`.
- `GET api/RegisterUser/GetTheCurrentUser`. Its only caller was checkout, which now takes the buyer from the token.
- `GET api/Misc/Test`, a liveness check.

Some slices sit in a different feature folder from their URL group:
- `Checkout/CheckoutCart` is under `api/Cart`.
- `Checkout/GetAllCheckouts` is under `api/Admin`.
- `Account/*` is split across `api/Registration`, `api/Login` and `api/Admin`.

These slices stay with the feature whose `Shared/` they use.

The `Checkouts` table is now `Orders`, and its entity `CheckoutInformation` is now `Domain/Ordering/Order`, because each
row is one paid purchase. The `RenameCheckoutsToOrders` migration renames the table, its primary key and the
`ShoppingCarts` foreign key in place, so every row is kept. EF scaffolds an entity rename as drop-and-create, so that
migration is hand-written and must stay a rename. Columns, `ShoppingCarts.CheckoutId` and the API's JSON
(`CheckoutDto`) are unchanged. No other schema change was made. `MigrationsAssembly` is `Resturant.Infrastructure`,
and migration IDs in `__EFMigrationsHistory` don't depend on the assembly.

## Known issues

- Card numbers and CVVs are stored and returned in plain text (`Order`, `SavedPaymentMethod`). That's a PCI-DSS
  concern to handle separately.
- `UserHistoryPayments` sorts dates as text, so "10/1" comes before "9/1".
