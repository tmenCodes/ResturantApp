using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;

namespace Domain.Tests;

public class OrderTests
{
    [Fact]
    public void PayFor_RecordsCardUserCartAndTimeAsPaid()
    {
        var cart = Guid.NewGuid();
        var paidAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var card = new CreditCardInformation("Visa", "Ada Lovelace", "4111", "12/30", "123");

        var checkout = Order.PayFor(cart, card, "ada", paidAt);

        Assert.True(checkout.Paid);
        Assert.Equal(paidAt, checkout.CheckoutTime);
        Assert.Equal(cart, checkout.ShoppingCartIdentity);
        Assert.Equal("ada", checkout.UserProfileName);
        Assert.Equal(card, checkout.Card);
    }
}
