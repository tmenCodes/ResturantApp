using API.Common;
using API.Features.Checkout.Shared;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Checkout.CheckoutCart;

/// <summary>Records payment for a shopping cart.</summary>
public class CheckoutCartHandler : IHandler
{
    private readonly RestaurantContext _context;
    private readonly TimeProvider _timeProvider;

    public CheckoutCartHandler(RestaurantContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Checks out the cart named by <see cref="CheckoutRequestDto.CartsIdentity"/>. The cart must exist and must
    /// not already be checked out. The checkout is recorded as paid at the current UTC time, for
    /// <paramref name="userName"/> (the signed-in user), or for nobody when it is null (a guest checkout).
    /// </summary>
    public async Task<CheckoutResult> HandleAsync(
        CheckoutRequestDto request, string? userName, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.CartsIdentity, out var cartIdentity))
        {
            return new CheckoutResult(CheckoutStatus.CartNotFound);
        }

        switch (await CheckoutEligibility.CheckAsync(_context, cartIdentity, cancellationToken))
        {
            case CheckoutEligibilityResult.CartAlreadyCheckedOut:
                return new CheckoutResult(CheckoutStatus.CartAlreadyCheckedOut);
            case CheckoutEligibilityResult.CartNotFound:
                return new CheckoutResult(CheckoutStatus.CartNotFound);
        }

        var card = new CreditCardInformation(request.Credit, request.NameonCard, request.CreditCardNumber, request.Expiration, request.CVV);
        var checkout = Order.PayFor(cartIdentity, card, userName, _timeProvider.GetUtcNow().UtcDateTime);

        _context.Orders.Add(checkout);
        await _context.SaveChangesAsync(cancellationToken);
        return new CheckoutResult(CheckoutStatus.Created, CheckoutDto.From(checkout));
    }
}
