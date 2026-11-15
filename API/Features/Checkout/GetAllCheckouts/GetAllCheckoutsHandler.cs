using API.Common;
using API.Features.Checkout.Shared;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Checkout.GetAllCheckouts;

/// <summary>Lists past checkouts.</summary>
public class GetAllCheckoutsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetAllCheckoutsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns every checkout, or an empty list when there are none.</summary>
    public async Task<IReadOnlyList<CheckoutDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var checkouts = await _context.Checkouts.AsNoTracking().ToListAsync(cancellationToken);
        return checkouts.Select(CheckoutDto.From).ToList();
    }
}
