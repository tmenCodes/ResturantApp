using API.Common;
using API.Features.Checkout.Shared;
using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;
using Resturant.Domain.Services.Ordering;
using CheckoutEntity = Resturant.Domain.Ordering.Checkout;

namespace API.Features.Checkout.CheckoutCart;

/// <summary>Records payment for a shopping cart.</summary>
public class CheckoutCartHandler : IHandler
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly ICheckoutEligibility _checkoutEligibility;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CheckoutCartHandler(ICheckoutRepository checkoutRepository, ICheckoutEligibility checkoutEligibility, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _checkoutRepository = checkoutRepository;
        _checkoutEligibility = checkoutEligibility;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Checks out the cart named by <see cref="CheckoutRequestDto.CartsIdentity"/>. The cart must exist and must
    /// not already be checked out. The checkout is recorded as paid at the current UTC time.
    /// </summary>
    public async Task<CheckoutResult> HandleAsync(CheckoutRequestDto request, CancellationToken cancellationToken = default)
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
        var checkout = CheckoutEntity.PayFor(cartIdentity, card, request.UserName, _timeProvider.GetUtcNow().UtcDateTime);
        await _checkoutRepository.AddAsync(checkout, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CheckoutResult(CheckoutStatus.Created, CheckoutDto.From(checkout));
    }
}
