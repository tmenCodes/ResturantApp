using API.Features.UserProfile.GetPaymentMethod;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;
using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class GetPaymentMethodHandlerTests
{
    private static GetPaymentMethodHandler CreateHandler(RestaurantContext context) => new(new UserProfileRepository(context));

    [Fact]
    public async Task HandleAsync_ReturnsOnlyThatUsersFirstPaymentMethod()
    {
        await using var context = TestDatabase.Create();
        var add = AddHandler(context);
        await add.HandleAsync("grace", Payment("Grace Hopper"));
        await add.HandleAsync("ada", Payment("First"));
        await add.HandleAsync("ada", Payment("Second"));

        var result = await CreateHandler(context).HandleAsync("ada");

        Assert.Equal("First", result!.NameonCard);
    }

    [Fact]
    public async Task HandleAsync_NoPaymentMethod_ReturnsNull()
    {
        await using var context = TestDatabase.Create();

        Assert.Null(await CreateHandler(context).HandleAsync("ada"));
    }
}
