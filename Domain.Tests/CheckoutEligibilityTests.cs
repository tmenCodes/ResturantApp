using Domain.Tests.Fakes;
using Resturant.Domain.Ordering;
using Resturant.Domain.Services.Ordering;

namespace Domain.Tests;

public class CheckoutEligibilityTests
{
    private static readonly DateTime Now = new(2026, 10, 28, 12, 0, 0, DateTimeKind.Utc);
    private static readonly PaymentCard Card = new("Visa", "Ada", "4111", "12/30", "123");

    private readonly FakeCheckoutRepository _checkouts = new();
    private readonly FakeShoppingCartRepository _carts = new();
    private readonly CheckoutEligibility _eligibility;

    public CheckoutEligibilityTests()
    {
        _eligibility = new CheckoutEligibility(_checkouts, _carts);
    }

    private Guid AddCart()
    {
        var identity = Guid.NewGuid();
        _carts.Carts.Add(ShoppingCart.Create(identity, Now));
        return identity;
    }

    [Fact]
    public async Task CheckAsync_ExistingUnpaidCart_IsEligible()
    {
        Assert.Equal(CheckoutEligibilityResult.Eligible, await _eligibility.CheckAsync(AddCart()));
    }

    [Fact]
    public async Task CheckAsync_UnknownCart_IsCartNotFound()
    {
        Assert.Equal(CheckoutEligibilityResult.CartNotFound, await _eligibility.CheckAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CheckAsync_AlreadyCheckedOutCart_IsCartAlreadyCheckedOut()
    {
        var cart = AddCart();
        _checkouts.Checkouts.Add(Checkout.PayFor(cart, Card, "ada", Now));

        Assert.Equal(CheckoutEligibilityResult.CartAlreadyCheckedOut, await _eligibility.CheckAsync(cart));
    }

    [Fact]
    public async Task CheckAsync_CheckedOutButCartMissing_StillReportsAlreadyCheckedOut()
    {
        var cart = Guid.NewGuid();
        _checkouts.Checkouts.Add(Checkout.PayFor(cart, Card, "ada", Now));

        Assert.Equal(CheckoutEligibilityResult.CartAlreadyCheckedOut, await _eligibility.CheckAsync(cart));
    }
}
