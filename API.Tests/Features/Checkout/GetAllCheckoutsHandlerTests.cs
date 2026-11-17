using API.Features.Checkout.GetAllCheckouts;
using static API.Tests.Features.Checkout.CheckoutTestData;

namespace API.Tests.Features.Checkout;

public class GetAllCheckoutsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsEveryCheckout()
    {
        await using var context = TestDatabase.Create();
        var checkout = CheckoutHandler(context);
        await checkout.HandleAsync(Request((await AddCartAsync(context)).ToString()), "ada");
        await checkout.HandleAsync(Request((await AddCartAsync(context)).ToString()), "ada");

        var all = await new GetAllCheckoutsHandler(context).HandleAsync();

        Assert.Equal(2, all.Count);
    }
}
