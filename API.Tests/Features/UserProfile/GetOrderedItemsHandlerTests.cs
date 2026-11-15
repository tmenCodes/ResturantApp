using API.Features.UserProfile.GetOrderedItems;
using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class GetOrderedItemsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsFormattedItemsFromTheUsersCheckedOutCarts()
    {
        await using var context = TestDatabase.Create();
        var checkoutTime = new DateTime(2026, 10, 28, 12, 0, 0);
        var cart = await AddCheckedOutCartAsync(context, "ada", checkoutTime, 10m);
        var otherCart = await AddCheckedOutCartAsync(context, "grace", checkoutTime, 10m);
        AddOrderItem(context, "Papaya Salad", 8.5m, cart);
        AddOrderItem(context, "Tofu Stir Fry", 12.95m, otherCart);
        await context.SaveChangesAsync();

        var result = await new GetOrderedItemsHandler(context).HandleAsync("ada");

        var item = Assert.Single(result);
        Assert.Equal(new UserOrderedItemDto(checkoutTime.ToString(), cart.ToString(), "Papaya Salad", "8.50"), item);
    }
}
