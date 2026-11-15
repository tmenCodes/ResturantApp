using API.Common;
using API.Features.UserProfile.Shared;
using Resturant.Domain.Repositories;

namespace API.Features.UserProfile.UpdatePaymentMethod;

/// <summary>Replaces the card on a user's saved payment method.</summary>
public class UpdatePaymentMethodHandler : IHandler
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentMethodHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Overwrites the user's saved payment method. Returns null when they have none to update.</summary>
    public async Task<SavedPaymentMethodDto?> HandleAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _userProfileRepository.GetPaymentMethodAsync(userName, cancellationToken);
        if (paymentMethod is null)
        {
            return null;
        }

        paymentMethod.UpdateCard(paymentInformation.ToCreditCardInformation());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return SavedPaymentMethodDto.From(paymentMethod);
    }
}
