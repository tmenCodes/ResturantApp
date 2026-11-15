using API.Common;
using API.Features.UserProfile.Shared;
using Resturant.Domain.Customers;
using Resturant.Infrastructure.Persistence;

namespace API.Features.UserProfile.AddPaymentMethod;

/// <summary>Saves a payment method to a user's profile.</summary>
public class AddPaymentMethodHandler : IHandler
{
    private readonly RestaurantContext _context;

    public AddPaymentMethodHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Saves a new payment method for the user and returns it.</summary>
    public async Task<SavedPaymentMethodDto> HandleAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = SavedPaymentMethod.Create(userName, paymentInformation.ToCreditCardInformation());
        await _context.SavedPaymentMethods.AddAsync(paymentMethod, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return SavedPaymentMethodDto.From(paymentMethod);
    }
}
