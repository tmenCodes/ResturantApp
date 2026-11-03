using Resturant.Application.Dtos;
using Resturant.Application.Mappings;
using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;
using Resturant.Domain.Services.Ordering;
using Resturant.Application.Interfaces;

namespace Resturant.Application.Services;

/// <summary>Default <see cref="ICheckoutService"/>.</summary>
public class CheckoutService : ICheckoutService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly ICheckoutEligibility _checkoutEligibility;
    private readonly TimeProvider _timeProvider;

    public CheckoutService(ICheckoutRepository checkoutRepository, ICheckoutEligibility checkoutEligibility, TimeProvider timeProvider)
    {
        _checkoutRepository = checkoutRepository;
        _checkoutEligibility = checkoutEligibility;
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

        switch (await _checkoutEligibility.CheckAsync(cartIdentity, cancellationToken))
        {
            case CheckoutEligibilityResult.CartAlreadyCheckedOut:
                return new CheckoutResult(CheckoutStatus.CartAlreadyCheckedOut);
            case CheckoutEligibilityResult.CartNotFound:
                return new CheckoutResult(CheckoutStatus.CartNotFound);
        }

        var card = new PaymentCard(request.Credit, request.NameonCard, request.CreditCardNumber, request.Expiration, request.CVV);
        var checkout = Checkout.PayFor(cartIdentity, card, request.UserName, _timeProvider.GetUtcNow().UtcDateTime);
        await _checkoutRepository.AddAsync(checkout, cancellationToken);
        return new CheckoutResult(CheckoutStatus.Created, checkout.ToDto());
    }
}
