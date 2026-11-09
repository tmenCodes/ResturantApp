using API.Common;
using Resturant.Infrastructure.Persistence.Queries;

namespace API.Features.UserProfile.GetOrderedItems;

/// <summary>Lists the items a user has ordered.</summary>
public class GetOrderedItemsHandler : IHandler
{
    private readonly IOrderHistoryQueries _orderHistoryQueries;

    public GetOrderedItemsHandler(IOrderHistoryQueries orderHistoryQueries)
    {
        _orderHistoryQueries = orderHistoryQueries;
    }

    /// <summary>Returns every item from every order the user has checked out.</summary>
    public async Task<IReadOnlyList<UserOrderedItemDto>> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var items = await _orderHistoryQueries.GetCheckedOutOrderItemsAsync(userName, cancellationToken);
        return items.Select(ToDto).ToList();
    }

    private static UserOrderedItemDto ToDto(CheckedOutOrderItem item) =>
        new(item.CheckoutTime.ToString(), item.ShoppingCartIdentity.ToString(), item.Name, item.TotalItemPrice.ToString("0.00"));
}
