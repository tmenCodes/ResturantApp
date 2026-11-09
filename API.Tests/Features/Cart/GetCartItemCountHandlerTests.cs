using API.Features.Cart.GetCartItemCount;
using Resturant.Infrastructure.Persistence.Repositories;
using static API.Tests.Features.Cart.CartTestData;

namespace API.Tests.Features.Cart;

public class GetCartItemCountHandlerTests
{
    [Fact]
    public async Task HandleAsync_CountsOnlyThisCartsLines()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await AddAsync(context, cartId);
        await AddAsync(context, Guid.NewGuid());

        Assert.Equal(1, await new GetCartItemCountHandler(new ShoppingCartRepository(context)).HandleAsync(cartId.ToString()));
    }

    [Fact]
    public async Task HandleAsync_MalformedIdentity_ReturnsZero()
    {
        await using var context = TestDatabase.Create();

        Assert.Equal(0, await new GetCartItemCountHandler(new ShoppingCartRepository(context)).HandleAsync("not-a-guid"));
    }
}
