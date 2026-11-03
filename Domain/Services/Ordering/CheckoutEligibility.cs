using Resturant.Domain.Repositories;

namespace Resturant.Domain.Services.Ordering;

/// <summary>Default <see cref="ICheckoutEligibility"/>, backed by the checkout and shopping cart repositories.</summary>
public class CheckoutEligibility : ICheckoutEligibility
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public CheckoutEligibility(ICheckoutRepository checkoutRepository, IShoppingCartRepository shoppingCartRepository)
    {
        _checkoutRepository = checkoutRepository;
        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task<CheckoutEligibilityResult> CheckAsync(Guid cartIdentity, CancellationToken cancellationToken = default)
    {
        if (await _checkoutRepository.ExistsForCartAsync(cartIdentity, cancellationToken))
        {
            return CheckoutEligibilityResult.CartAlreadyCheckedOut;
        }

        return await _shoppingCartRepository.GetCartAsync(cartIdentity) is null
            ? CheckoutEligibilityResult.CartNotFound
            : CheckoutEligibilityResult.Eligible;
    }
}
