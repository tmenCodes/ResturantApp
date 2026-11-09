using API.Features.UserProfile.UpdatePaymentMethod;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;
using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class UpdatePaymentMethodHandlerTests
{
    private static UpdatePaymentMethodHandler CreateHandler(RestaurantContext context) =>
        new(new UserProfileRepository(context), new EfUnitOfWork(context));

    [Fact]
    public async Task HandleAsync_ExistingPaymentMethod_OverwritesIt()
    {
        await using var context = TestDatabase.Create();
        await AddHandler(context).HandleAsync("ada", Payment("Old Name"));

        var result = await CreateHandler(context).HandleAsync("ada", Payment("New Name"));

        Assert.Equal("New Name", result!.NameonCard);
        Assert.Equal("New Name", Assert.Single(context.SavedPaymentMethods).NameonCard);
    }

    [Fact]
    public async Task HandleAsync_NoPaymentMethod_ReturnsNullAndSavesNothing()
    {
        await using var context = TestDatabase.Create();

        var result = await CreateHandler(context).HandleAsync("ada", Payment("Ada Lovelace"));

        Assert.Null(result);
        Assert.Empty(context.SavedPaymentMethods);
    }
}
