using API.Common;
using API.Features.UserProfile.Shared;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.UserProfile.UpdatePaymentMethod;

/// <summary>Replaces the card on a user's saved payment method.</summary>
public class UpdatePaymentMethodHandler : IHandler
{
    private readonly RestaurantContext _context;

    public UpdatePaymentMethodHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Overwrites the user's first saved payment method (lowest id). Returns null when they have none to update.</summary>
    public async Task<SavedPaymentMethodDto?> HandleAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _context.SavedPaymentMethods
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (paymentMethod is null)
        {
            return null;
        }

        paymentMethod.UpdateCard(paymentInformation.ToCreditCardInformation());
        await _context.SaveChangesAsync(cancellationToken);
        return SavedPaymentMethodDto.From(paymentMethod);
    }
}
