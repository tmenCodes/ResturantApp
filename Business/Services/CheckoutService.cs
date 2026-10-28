using Resturant.Business.Dtos;
using Resturant.Business.Mappings;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Resturant.Business.Services;

/// <summary>Default <see cref="ICheckoutService"/>.</summary>
public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly TimeProvider _timeProvider;

    public CheckoutService(ICheckoutRepository checkoutRepository, IShoppingCartRepository shoppingCartRepository, TimeProvider timeProvider)
    {
        _checkoutRepository = checkoutRepository;
        _shoppingCartRepository = shoppingCartRepository;
        _timeProvider = timeProvider;
    }

    public async Task<IReadOnlyList<CheckoutDto>> GetAllCheckoutsAsync(CancellationToken cancellationToken = default)
    {
        var checkouts = await _checkoutRepository.GetAllAsync(cancellationToken);
        return checkouts.Select(x => x.ToDto()).ToList();
    }

    public async Task<CheckoutResult> CheckoutCartAsync(CheckoutRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(request.CartsIdentity, out var cartIdentity))
        {
            return new CheckoutResult(CheckoutStatus.CartNotFound);
        }

        if (await _checkoutRepository.ExistsForCartAsync(cartIdentity, cancellationToken))
        {
            return new CheckoutResult(CheckoutStatus.CartAlreadyCheckedOut);
        }

        if (await _shoppingCartRepository.GetCartAsync(cartIdentity.ToString()) is null)
        {
            return new CheckoutResult(CheckoutStatus.CartNotFound);
        }

        var checkout = new Checkout
        {
            Credit = request.Credit,
            NameonCard = request.NameonCard,
            CreditCardNumber = request.CreditCardNumber,
            Expiration = request.Expiration,
            CVV = request.CVV,
            UserProfileName = request.UserName,
            ShoppingCartIdentity = cartIdentity,
            CheckoutTime = _timeProvider.GetUtcNow().UtcDateTime,
            Paid = true
        };
        await _checkoutRepository.AddAsync(checkout, cancellationToken);
        return new CheckoutResult(CheckoutStatus.Created, checkout.ToDto());
    }
}
