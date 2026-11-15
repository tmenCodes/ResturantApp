using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Resturant.Infrastructure.Persistence;

namespace API.Tests.Integration;

/// <summary>
/// Runs the real API in memory, as production does, but on a fresh EF Core in-memory database (with seed data) and a
/// fixed clock. Each instance has its own database.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>The fixed current time, truncated to whole seconds and close enough to real time for JWT lifetimes.</summary>
    public static readonly DateTimeOffset Now = DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    private readonly string _databaseName = Guid.NewGuid().ToString();

    /// <summary>Starts the API, creates its seeded database and returns a client for it.</summary>
    public static async Task<(ApiFactory Api, HttpClient Client)> StartAsync()
    {
        var api = new ApiFactory();
        var client = api.CreateClient();
        await using var scope = api.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<RestaurantContext>().Database.EnsureCreatedAsync();
        return (api, client);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Production);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<RestaurantContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<RestaurantContext>>();
            services.AddDbContext<RestaurantContext>(options => options.UseInMemoryDatabase(_databaseName));
            services.AddSingleton<TimeProvider>(new FixedTimeProvider(Now));
        });
    }
}
