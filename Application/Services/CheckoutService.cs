using Resturant.Application.Dtos;
using Resturant.Application.Mappings;
using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;
using Resturant.Application.Interfaces;

namespace Resturant.Application.Services;

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

        var card = new PaymentCard(request.Credit, request.NameonCard, request.CreditCardNumber, request.Expiration, request.CVV);
        var checkout = Checkout.PayFor(cartIdentity, card, request.UserName, _timeProvider.GetUtcNow().UtcDateTime);
        await _checkoutRepository.AddAsync(checkout, cancellationToken);
        return new CheckoutResult(CheckoutStatus.Created, checkout.ToDto());
    }
}
