# Architecture

## Tiers

ResturantApp is a 3-tier application. Each tier is deployed and run separately:

| Tier | Technology | Runs on |
|---|---|---|
| Presentation | Angular SPA (`RestaurantFrontend` repo) | Browser |
| Application | ASP.NET Core Web API (this repo) | EC2 |
| Data | PostgreSQL | Database server |

## Onion architecture inside the API

The API follows **onion architecture**. Code is arranged in concentric rings, and **every dependency points
inward**. The core rings hold the business model and rules and know nothing about databases, HTTP or identity
providers. The outer ring plugs technology in by implementing interfaces that the inner rings define.

```
┌──────────────────────────────────────────────────────────────────┐
│  Outer ring: API (controllers, Program.cs)                        │
│              Infrastructure (EF Core/Postgres, Identity, JWT)     │
│              Tests                                                │
│   ┌──────────────────────────────────────────────────────────┐   │
│   │  Application Services: use cases, DTOs,                  │   │
│   │  abstractions for identity and tokens                    │   │
│   │   ┌──────────────────────────────────────────────────┐   │   │
│   │   │  Domain Services: rules that span entities and   │   │   │
│   │   │  need repositories                               │   │   │
│   │   │   ┌──────────────────────────────────────────┐   │   │   │
│   │   │   │  Domain Model: entities, their rules,    │   │   │   │
│   │   │   │  repository interfaces                   │   │   │   │
│   │   │   └──────────────────────────────────────────┘   │   │   │
│   │   └──────────────────────────────────────────────────┘   │   │
│   └──────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

| Ring | Project | Namespace root | Responsibility | May reference |
|---|---|---|---|---|
| Domain Model | `Domain` | `Resturant.Domain` | Entities and their own rules (cart totals, item pricing, checkout creation); repository interfaces in `Repositories/` | nothing |
| Domain Services | `Domain` (`Services/` folder) | `Resturant.Domain.Services` | Domain rules that span entities or need a repository: building an order item from the menu, checkout eligibility. May use only entities and repository interfaces | nothing outside `Domain` |
| Application Services | `Application` | `Resturant.Application` | Use cases (`Services/`) behind interfaces (`Interfaces/`), DTOs, mappings, and abstractions for non-domain concerns (`Abstractions/`: user accounts, tokens) | `Domain`, `Microsoft.Extensions.DependencyInjection.Abstractions` (for `AddApplication()`) |
| Outer | `Infrastructure` | `Resturant.Infrastructure` | EF Core context, entity configurations, migrations, seed data, repository implementations, ASP.NET Identity account store, JWT issuer | `Application`, `Domain`, EF Core, Npgsql, Identity |
| Outer | `API` | `API` | HTTP: routing, model binding, auth attributes, status codes. `Program.cs` is the composition root | `Application`; `Infrastructure` only from `Program.cs` |

### Rules

1. **Dependencies point inward only.** A ring may use the rings inside it, never the ones outside it.
2. **The Domain Model is plain C#.** It has no EF Core, ASP.NET, JSON or data-annotation attributes. EF maps the
   entities through `IEntityTypeConfiguration<T>` classes in `Infrastructure/Persistence/Configurations`.
   Entities have no public setters and expose collections as read-only; they change only through their own
   constructors and methods (`ShoppingCart.AddItem`, `SavedPaymentMethod.UpdateCard`, ...), so rules like cart totals
   can't be bypassed. EF uses their private setters and backing fields. `Domain.Tests/EncapsulationTests` guards this.
3. **Repository interfaces belong to the Domain Model** (`Resturant.Domain.Repositories`). Infrastructure implements them.
   Repositories only read and stage changes. Application Services commit once per request through `IUnitOfWork`,
   so related changes are saved together.
4. **Rules live as far inward as possible.** A rule about one entity is a method on that entity. A rule that needs
   several entities or a repository is a Domain Service. Application Services only orchestrate: load, call the
   domain, save, map to DTOs.
5. **Reports are queries, not repositories.** Read-only views that join several tables (order history) are
   Application query interfaces returning read models, implemented in `Infrastructure/Persistence/Queries`.
   Domain repositories return entities only.
6. **Non-domain concerns are Application abstractions.** `IUserAccountStore` and `ITokenIssuer` live in
   `Resturant.Application.Abstractions`; `UserManager`, `WebUser`, `IdentityError` and `JwtSecurityTokenHandler`
   exist only in Infrastructure.
7. **Controllers depend only on `Resturant.Application.Interfaces`** and translate results into HTTP responses.
8. **Entities never leave the Application ring.** Application interfaces take and return DTOs whose property
   names match the entities, so **the JSON the Angular app receives does not change**.
9. **Composition is idempotent.** `AddApplication()` (which also registers Domain Services) and
   `AddInfrastructure(...)` register with `TryAdd*`.
10. **Domain Services share the Domain project.** They are a separate ring by convention, in `Domain/Services`,
   rather than a separate project. They must not depend on the Application or outer rings, and entities must not
   depend on them.
11. **Rings are tested.** `Application.Tests/ArchitectureTests` fails if an inner ring gains a reference to an outer
   ring or to a framework.

## Project layout

```
Domain/
  Menu/                 MenuItem, MenuItemOption
  Ordering/             ShoppingCart, OrderItem, OrderItemOption, Checkout, PaymentCard
  Customers/            SavedPaymentMethod
  Contact/              Reservation, CustomerInquiry
  Repositories/         IUnitOfWork, IMenuRepository, IShoppingCartRepository, ICheckoutRepository,
                        IContactRepository, IUserProfileRepository
  Services/Ordering/    Domain Services: IOrderItemFactory, OrderItemFactory, ICheckoutEligibility, CheckoutEligibility
Application/
  Interfaces/           IMenuService, ICartService, ICheckoutService, IContactService,
                        IUserProfileService, IAccountService
  Services/             one implementation per interface
  Abstractions/         IUserAccountStore, ITokenIssuer, UserAccount, ...; read-side queries
                        IOrderHistoryQueries with read models CheckedOutCart, CheckedOutOrderItem
  Dtos/, Mappings/
  ApplicationServiceCollectionExtensions.cs      AddApplication()
Infrastructure/
  Persistence/          RestaurantContext, EfUnitOfWork, Configurations/, Repositories/, Queries/, Migrations/
  Identity/             WebUser, IdentityUserAccountStore, IdentityServiceCollectionExtensions
  Auth/                 JwtSettings, JwtTokenIssuer
  InfrastructureServiceCollectionExtensions.cs   AddInfrastructure()
API/
  Controllers/, Contracts/, Program.cs
Domain.Tests/           entity rules and Domain Services (with in-memory fake repositories)
Application.Tests/      application services and Infrastructure (xUnit + EF Core InMemory), architecture tests
```

## Service catalog

| Controller (routes unchanged) | Application service | Domain services | Repositories / abstractions |
|---|---|---|---|
| `OrderController`, menu endpoints | `MenuService` | | `IMenuRepository` |
| `OrderController`, cart endpoints | `CartService` | `IOrderItemFactory` | `IShoppingCartRepository` |
| `PaymentController` | `CheckoutService` | `ICheckoutEligibility` | `ICheckoutRepository` |
| `UserInformationController` | `UserProfileService` | | `IUserProfileRepository`, `IOrderHistoryQueries` |
| `RegisterUser` | `AccountService` | | `IUserAccountStore`, `ITokenIssuer` |
| `MiscController` | `ContactService` | | `IContactRepository` |

## Unchanged contracts

- Every route, HTTP method and JSON request/response shape stays the same.
- The database schema stays the same. `MigrationsAssembly` is `Resturant.Infrastructure`, and migration IDs in
  `__EFMigrationsHistory` don't depend on the assembly, so nothing needs re-applying.

## Known issues

- Card numbers and CVVs are stored and returned in plain text (`Checkout`, `SavedPaymentMethod`). That's a PCI-DSS
  concern to handle separately.
- `UserHistoryPayments` sorts dates as text, so "10/1" comes before "9/1".
