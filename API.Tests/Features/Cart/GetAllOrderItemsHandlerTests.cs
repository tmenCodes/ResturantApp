using API.Features.Cart.GetAllOrderItems;
using Resturant.Infrastructure.Persistence.Repositories;
using static API.Tests.Features.Cart.CartTestData;

namespace API.Tests.Features.Cart;

public class GetAllOrderItemsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsLinesFromEveryCart()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        await AddAsync(context, Guid.NewGuid());
        await AddAsync(context, Guid.NewGuid(), 2);

        var items = await new GetAllOrderItemsHandler(new ShoppingCartRepository(context)).HandleAsync();

        Assert.Equal(2, items.Count);
        Assert.All(items, item => Assert.Null(item.GuidId));
        Assert.Equal(new[] { 10m, 20m }, items.Select(x => x.Price).Order());
    }
}
