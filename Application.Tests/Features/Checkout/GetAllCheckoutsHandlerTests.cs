using API.Features.Checkout.GetAllCheckouts;
using Resturant.Infrastructure.Persistence.Repositories;
using static Application.Tests.Features.Checkout.CheckoutTestData;

namespace Application.Tests.Features.Checkout;

public class GetAllCheckoutsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsEveryCheckout()
    {
        await using var context = TestDatabase.Create();
        var checkout = CheckoutHandler(context);
        await checkout.HandleAsync(Request((await AddCartAsync(context)).ToString()));
        await checkout.HandleAsync(Request((await AddCartAsync(context)).ToString()));

        var all = await new GetAllCheckoutsHandler(new CheckoutRepository(context)).HandleAsync();

        Assert.Equal(2, all.Count);
    }
}
