using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Checkout.CheckoutCart;

/// <summary>Whether a shopping cart may be checked out, and why not.</summary>
internal enum CheckoutEligibilityResult
{
    Eligible,
    CartNotFound,
    CartAlreadyCheckedOut
}

/// <summary>Decides whether a shopping cart may be checked out.</summary>
internal static class CheckoutEligibility
{
    /// <summary>
    /// Returns <see cref="CheckoutEligibilityResult.Eligible"/> when the cart exists and has not been checked out yet.
    /// An already checked-out cart is reported as such even if the cart itself no longer exists.
    /// </summary>
    public static async Task<CheckoutEligibilityResult> CheckAsync(RestaurantContext context, Guid cartIdentity, CancellationToken cancellationToken)
    {
        if (await context.Orders.AnyAsync(x => x.ShoppingCartIdentity == cartIdentity, cancellationToken))
        {
            return CheckoutEligibilityResult.CartAlreadyCheckedOut;
        }

        return await context.ShoppingCarts.AnyAsync(x => x.Identity == cartIdentity, cancellationToken)
            ? CheckoutEligibilityResult.Eligible
            : CheckoutEligibilityResult.CartNotFound;
    }
}
