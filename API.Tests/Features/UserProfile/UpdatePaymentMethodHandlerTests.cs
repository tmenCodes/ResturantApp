using API.Features.UserProfile.UpdatePaymentMethod;
using Resturant.Infrastructure.Persistence;
using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class UpdatePaymentMethodHandlerTests
{
    private static UpdatePaymentMethodHandler CreateHandler(RestaurantContext context) =>
        new(context);

    [Fact]
    public async Task HandleAsync_ExistingPaymentMethod_OverwritesIt()
    {
        await using var context = TestDatabase.Create();
        await AddHandler(context).HandleAsync("ada", Payment("Old Name"));

        var result = await CreateHandler(context).HandleAsync("ada", Payment("New Name"));

        Assert.Equal("New Name", result!.NameonCard);
        context.ChangeTracker.Clear();
        Assert.Equal("New Name", Assert.Single(context.SavedPaymentMethods).Card.NameOnCard);
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
