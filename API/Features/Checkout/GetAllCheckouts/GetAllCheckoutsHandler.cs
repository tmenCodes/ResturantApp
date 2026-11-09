using API.Common;
using API.Features.Checkout.Shared;
using Resturant.Domain.Repositories;

namespace API.Features.Checkout.GetAllCheckouts;

/// <summary>Lists past checkouts.</summary>
public class GetAllCheckoutsHandler : IHandler
{
    private readonly ICheckoutRepository _checkoutRepository;

    public GetAllCheckoutsHandler(ICheckoutRepository checkoutRepository)
    {
        _checkoutRepository = checkoutRepository;
    }

    /// <summary>Returns every checkout, or an empty list when there are none.</summary>
    public async Task<IReadOnlyList<CheckoutDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var checkouts = await _checkoutRepository.GetAllAsync(cancellationToken);
        return checkouts.Select(CheckoutDto.From).ToList();
    }
}
