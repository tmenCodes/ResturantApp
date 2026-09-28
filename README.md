# ResturantApp

A restaurant management API built with ASP.NET Core, following a clean architecture layout (Domain / Application / Infrastructure / API).

## Tech Stack

- **.NET 9** (ASP.NET Core Web API)
- **Entity Framework Core** with **PostgreSQL** (Npgsql)
- **ASP.NET Core Identity** + **JWT Bearer** authentication
- **Hangfire** for background jobs
- **Swagger / Swashbuckle** for API documentation
- **MediatR** for request/response handling

## Project Structure

```
ResturantApp/
├── API/                  # Web API entry point, controllers, configuration
├── Application/          # Application logic, services, use cases
├── Domain/               # Domain entities and core business rules
├── Infrastructure/       # EF Core context, migrations, external services
├── OrderControllerTest/  # Integration tests
├── TestProject1/         # Unit tests
├── UnitTesting1/         # Unit tests
└── Resturant.sln         # Solution file
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (running locally or accessible remotely)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ResturantApp
   ```

2. **Configure application settings**

   Update `API/appsettings.json` with your local `ConnectionStrings:DefaultConnection` and `JwtSettings:Secret` values. **Do not commit real credentials.**

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update --project Infrastructure --startup-project API
   ```

5. **Run the API**
   ```bash
   dotnet run --project API
   ```

6. **Browse the API docs**

   Once running in development mode, Swagger UI is available at `/swagger`.

## Running Tests

```bash
dotnet test
```

## Deployment

This repository includes a GitHub Actions workflow (`.github/workflows/aws.yml`) that builds the app and deploys it to an EC2 instance on pushes to `main`. It expects the following repository secrets to be configured under **Settings → Secrets and variables → Actions**:

- `EC2_HOST`
- `EC2_USERNAME`
- `EC2_SSH_KEY`

## Security Notes

- Never commit real database credentials, JWT secrets, or API keys.
- If a secret has ever been committed to this repository, treat it as compromised, rotate it, and consider rewriting git history to remove it.

## License

TBD
