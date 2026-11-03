# Architecture

## Tiers

ResturantApp is a 3-tier application. Each tier is deployed and run separately:

| Tier | Technology | Runs on |
|---|---|---|
| Presentation | Angular SPA (`RestaurantFrontend` repo) | Browser |
| Application | ASP.NET Core Web API (this repo) | EC2 |
| Data | PostgreSQL | Database server |

## Hexagonal architecture inside the API

The API follows **hexagonal architecture** (ports and adapters). The core (Domain + Application) holds every
business rule and depends on nothing technical. It talks to the outside world only through **ports**
(interfaces it owns). **Adapters** implement or call those ports.

```
              inbound adapter                       outbound adapters
          ┌──────────────────┐                ┌───────────────────────────┐
 HTTP ──► │ API (controllers)│ ──► Ports/In   │ Infrastructure            │
          └──────────────────┘     │          │  Persistence (EF/Postgres)│
                                   ▼          │  Identity (UserManager)   │
                          ┌─────────────────┐ │  Auth (JWT)               │
                          │ Application     │ └───────────────────────────┘
                          │  UseCases       │──► Ports/Out ◄── implemented by Infrastructure
                          └────────┬────────┘
                                   ▼
                          ┌─────────────────┐
                          │ Domain          │  entities + business rules
                          └─────────────────┘
```

| Project | Namespace root | Responsibility | May reference |
|---|---|---|---|
| `Domain` | `Resturant.Domain` | Entities and their rules (cart totals, item pricing, checkout creation) | nothing |
| `Application` | `Resturant.Application` | Use cases, inbound ports (`Ports/In`), outbound ports (`Ports/Out`), DTOs | `Domain` |
| `Infrastructure` | `Resturant.Infrastructure` | Adapters for the outbound ports: EF Core context, entity configurations, migrations, seed data, repositories, ASP.NET Identity store, JWT issuer | `Application`, `Domain`, EF Core, Npgsql, Identity |
| `API` | `API` | Inbound HTTP adapter: routing, model binding, auth attributes, status codes. `Program.cs` is the composition root | `Application`; `Infrastructure` only from `Program.cs` |

### Rules

1. **The Domain is plain C#.** It has no EF Core, ASP.NET, JSON or data-annotation attributes. EF maps the domain
   classes through `IEntityTypeConfiguration<T>` classes in `Infrastructure/Persistence/Configurations`.
2. **The Application owns its ports.** Inbound ports (`I<Name>Service`) are what the API calls. Outbound ports
   (`I<Name>Repository`, `IUserAccountStore`, `ITokenIssuer`) are what the use cases need from the outside world.
3. **The Application never references ASP.NET Core or Identity.** `UserManager`, `WebUser`, `IdentityError` and
   `JwtSecurityTokenHandler` exist only in Infrastructure, behind `IUserAccountStore` and `ITokenIssuer`.
4. **Business rules live in the Domain.** Use cases load entities through ports, call domain methods, and save.
5. **Controllers depend only on inbound ports** and translate results into HTTP responses.
6. **Entities never leave the Application.** Inbound ports take and return DTOs whose property names match the
   entities, so **the JSON the Angular app receives does not change**.
7. **Composition is idempotent.** `AddApplication()` and `AddInfrastructure(...)` register with `TryAdd*`.
8. **Boundaries are tested.** `Application.Tests/ArchitectureTests` fails the build if Domain or Application gains
   a forbidden assembly reference.

## Project layout

```
Domain/
  Menu/                 MenuItem, MenuItemOption
  Ordering/             ShoppingCart, OrderItem, OrderItemOption, Checkout
  Customers/            SavedPaymentMethod
  Contact/              Reservation, CustomerInquiry
Application/
  Ports/In/             IMenuService, ICartService, ICheckoutService, IContactService,
                        IUserProfileService, IAccountService
  Ports/Out/            IMenuRepository, IShoppingCartRepository, ICheckoutRepository, IContactRepository,
                        IUserProfileRepository, IUserAccountStore, ITokenIssuer
  UseCases/             one implementation per inbound port
  Dtos/, Mappings/
  ApplicationServiceCollectionExtensions.cs      AddApplication()
Infrastructure/
  Persistence/          RestaurantContext, Configurations/, Repositories/, Migrations/
  Identity/             WebUser, IdentityUserAccountStore, IdentityServiceCollectionExtensions
  Auth/                 JwtSettings, JwtTokenIssuer
  InfrastructureServiceCollectionExtensions.cs   AddInfrastructure()
API/
  Controllers/, Contracts/, Program.cs
Domain.Tests/           pure unit tests of entity rules
Application.Tests/      use cases and adapters (xUnit + EF Core InMemory), architecture tests
```

## Use case and port catalog

| Controller (routes unchanged) | Inbound port / use case | Outbound ports |
|---|---|---|
| `OrderController`, menu endpoints | `IMenuService` / `MenuService` | `IMenuRepository` |
| `OrderController`, cart endpoints | `ICartService` / `CartService` | `IShoppingCartRepository`, `IMenuRepository` |
| `PaymentController` | `ICheckoutService` / `CheckoutService` | `ICheckoutRepository`, `IShoppingCartRepository` |
| `UserInformationController` | `IUserProfileService` / `UserProfileService` | `IUserProfileRepository` |
| `RegisterUser` | `IAccountService` / `AccountService` | `IUserAccountStore`, `ITokenIssuer` |
| `MiscController` | `IContactService` / `ContactService` | `IContactRepository` |

## Unchanged contracts

- Every route, HTTP method and JSON request/response shape stays the same.
- The database schema stays the same. `MigrationsAssembly` is `Resturant.Infrastructure`, and migration IDs in
  `__EFMigrationsHistory` don't depend on the assembly, so nothing needs re-applying.

## Known issues

- Card numbers and CVVs are stored and returned in plain text (`Checkout`, `SavedPaymentMethod`). That's a PCI-DSS
  concern to handle separately.
- `UserHistoryPayments` sorts dates as text, so "10/1" comes before "9/1".
