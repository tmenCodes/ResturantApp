using API.Common;
using API.Features.UserProfile.Shared;
using Resturant.Domain.Repositories;

namespace API.Features.UserProfile.GetPaymentMethod;

/// <summary>Reads a user's saved payment method.</summary>
public class GetPaymentMethodHandler : IHandler
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetPaymentMethodHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    /// <summary>Returns the user's saved payment method, or null when they have none.</summary>
    public async Task<SavedPaymentMethodDto?> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _userProfileRepository.GetPaymentMethodAsync(userName, cancellationToken);
        return paymentMethod is null ? null : SavedPaymentMethodDto.From(paymentMethod);
    }
}
