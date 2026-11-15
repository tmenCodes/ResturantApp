using API.Features.Cart.GetCartItems;
using static API.Tests.Features.Cart.CartTestData;

namespace API.Tests.Features.Cart;

public class GetCartItemsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsOnlyThisCartsLinesWithOptions()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await AddAsync(context, cartId, 1, "Extra Tofu");
        await AddAsync(context, Guid.NewGuid());

        var items = await new GetCartItemsHandler(context).HandleAsync(cartId.ToString());

        var item = Assert.Single(items);
        Assert.Equal(cartId, item.ShoppingCartIdentity);
        Assert.Equal("Extra Tofu", Assert.Single(item.Options).Name);
    }

    [Fact]
    public async Task HandleAsync_MalformedIdentity_ReturnsEmpty()
    {
        await using var context = TestDatabase.Create();

        Assert.Empty(await new GetCartItemsHandler(context).HandleAsync("not-a-guid"));
    }
}
