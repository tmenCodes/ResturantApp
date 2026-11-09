using API.Features.Cart.GetCartTotals;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;
using static Application.Tests.Features.Cart.CartTestData;

namespace Application.Tests.Features.Cart;

public class GetCartTotalsHandlerTests
{
    private static GetCartTotalsHandler CreateHandler(RestaurantContext context) => new(new ShoppingCartRepository(context));

    [Fact]
    public async Task HandleAsync_FormatsAmountsWithTwoDecimals()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await AddAsync(context, cartId, 2, "Extra Tofu", "Soy Sauce");

        var totals = await CreateHandler(context).HandleAsync(cartId.ToString());

        Assert.NotNull(totals);
        Assert.Equal(23m.ToString("0.00"), totals.SubTotal);
        Assert.Equal((23m * TaxRate).ToString("0.00"), totals.TaxAmount);
        Assert.Equal((23m + 23m * TaxRate).ToString("0.00"), totals.TotalPrice);
    }

    [Fact]
    public async Task HandleAsync_MalformedIdentity_ReturnsNull()
    {
        await using var context = TestDatabase.Create();

        Assert.Null(await CreateHandler(context).HandleAsync("not-a-guid"));
    }

    [Fact]
    public async Task HandleAsync_UpperCaseIdentity_FindsCart()
    {
        await using var context = TestDatabase.Create();
        await SeedMenuAsync(context);
        var cartId = Guid.NewGuid();
        await AddAsync(context, cartId);

        Assert.NotNull(await CreateHandler(context).HandleAsync(cartId.ToString().ToUpperInvariant()));
    }

    [Fact]
    public async Task HandleAsync_UnknownCart_ReturnsNull()
    {
        await using var context = TestDatabase.Create();

        Assert.Null(await CreateHandler(context).HandleAsync(Guid.NewGuid().ToString()));
    }
}
