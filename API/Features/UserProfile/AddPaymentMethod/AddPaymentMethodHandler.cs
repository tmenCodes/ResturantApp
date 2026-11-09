using API.Common;
using API.Features.UserProfile.Shared;
using Resturant.Domain.Customers;
using Resturant.Domain.Repositories;

namespace API.Features.UserProfile.AddPaymentMethod;

/// <summary>Saves a payment method to a user's profile.</summary>
public class AddPaymentMethodHandler : IHandler
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddPaymentMethodHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>Saves a new payment method for the user and returns it.</summary>
    public async Task<SavedPaymentMethodDto> HandleAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = SavedPaymentMethod.Create(userName, paymentInformation.ToPaymentCard());
        await _userProfileRepository.AddPaymentMethodAsync(paymentMethod, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return SavedPaymentMethodDto.From(paymentMethod);
    }
}
