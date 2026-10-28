using Microsoft.EntityFrameworkCore;
using Resturant.Data.Context;

namespace Business.Tests;

/// <summary>Creates an isolated in-memory <see cref="RestaurantContext"/> per test.</summary>
public static class TestDatabase
{
    /// <summary>Returns a context backed by a new, uniquely named in-memory database.</summary>
    public static RestaurantContext Create()
    {
        var options = new DbContextOptionsBuilder<RestaurantContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RestaurantContext(options);
    }
}
