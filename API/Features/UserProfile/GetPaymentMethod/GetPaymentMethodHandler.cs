using API.Common;
using API.Features.UserProfile.Shared;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.UserProfile.GetPaymentMethod;

/// <summary>Reads a user's saved payment method.</summary>
public class GetPaymentMethodHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetPaymentMethodHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns the user's first saved payment method (lowest id), or null when they have none.</summary>
    public async Task<SavedPaymentMethodDto?> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _context.SavedPaymentMethods
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        return paymentMethod is null ? null : SavedPaymentMethodDto.From(paymentMethod);
    }
}
