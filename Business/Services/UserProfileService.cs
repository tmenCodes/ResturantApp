using Resturant.Business.Dtos;
using Resturant.Business.Mappings;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Resturant.Business.Services;

/// <summary>Default <see cref="IUserProfileService"/>.</summary>
public class UserProfileService : IUserProfileService
{
    private const int RecentOrderLimit = 5;

    private readonly IUserProfileRepository _userProfileRepository;

    public UserProfileService(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<SavedPaymentMethodDto> AddPaymentMethodAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = new SavedPaymentMethod { UserName = userName };
        paymentInformation.ApplyTo(paymentMethod);
        await _userProfileRepository.AddPaymentMethodAsync(paymentMethod, cancellationToken);
        return paymentMethod.ToDto();
    }

    public async Task<SavedPaymentMethodDto?> GetPaymentMethodAsync(string userName, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _userProfileRepository.GetPaymentMethodAsync(userName, cancellationToken);
        return paymentMethod?.ToDto();
    }

    public async Task<SavedPaymentMethodDto?> UpdatePaymentMethodAsync(string userName, UserPaymentInformationDto paymentInformation, CancellationToken cancellationToken = default)
    {
        var paymentMethod = await _userProfileRepository.GetPaymentMethodAsync(userName, cancellationToken);
        if (paymentMethod is null)
        {
            return null;
        }

        paymentInformation.ApplyTo(paymentMethod);
        await _userProfileRepository.UpdatePaymentMethodAsync(paymentMethod, cancellationToken);
        return paymentMethod.ToDto();
    }

    public async Task<IReadOnlyList<UserOrderHistoryDto>> GetRecentOrderTotalsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var carts = await _userProfileRepository.GetCheckedOutCartsAsync(userName, cancellationToken);
        return carts
            .Select(cart => cart.ToHistoryDto())
            .OrderBy(history => history.Date)
            .Take(RecentOrderLimit)
            .ToList();
    }

    public async Task<IReadOnlyList<UserOrderedItemDto>> GetOrderedItemsAsync(string userName, CancellationToken cancellationToken = default)
    {
        var items = await _userProfileRepository.GetCheckedOutOrderItemsAsync(userName, cancellationToken);
        return items.Select(item => item.ToDto()).ToList();
    }
}
