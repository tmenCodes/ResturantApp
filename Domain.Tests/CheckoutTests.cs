using Resturant.Domain.Ordering;

namespace Domain.Tests;

public class CheckoutTests
{
    [Fact]
    public void PayFor_RecordsCardUserCartAndTimeAsPaid()
    {
        var cart = Guid.NewGuid();
        var paidAt = new DateTime(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
        var card = new PaymentCard("Visa", "Ada Lovelace", "4111", "12/30", "123");

        var checkout = Checkout.PayFor(cart, card, "ada", paidAt);

        Assert.True(checkout.Paid);
        Assert.Equal(paidAt, checkout.CheckoutTime);
        Assert.Equal(cart, checkout.ShoppingCartIdentity);
        Assert.Equal("ada", checkout.UserProfileName);
        Assert.Equal("Visa", checkout.Credit);
        Assert.Equal("Ada Lovelace", checkout.NameonCard);
        Assert.Equal("4111", checkout.CreditCardNumber);
        Assert.Equal("12/30", checkout.Expiration);
        Assert.Equal("123", checkout.CVV);
    }
}
