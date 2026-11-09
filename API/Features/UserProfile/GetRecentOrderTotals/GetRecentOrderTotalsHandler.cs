using API.Common;
using Resturant.Infrastructure.Persistence.Queries;

namespace API.Features.UserProfile.GetRecentOrderTotals;

/// <summary>Lists a user's recent order totals.</summary>
public class GetRecentOrderTotalsHandler : IHandler
{
    private const int RecentOrderLimit = 5;

    private readonly IOrderHistoryQueries _orderHistoryQueries;

    public GetRecentOrderTotalsHandler(IOrderHistoryQueries orderHistoryQueries)
    {
        _orderHistoryQueries = orderHistoryQueries;
    }

    /// <summary>Returns up to five of the user's past order totals, ordered by their short-date text.</summary>
    public async Task<IReadOnlyList<UserOrderHistoryDto>> HandleAsync(string userName, CancellationToken cancellationToken = default)
    {
        var carts = await _orderHistoryQueries.GetCheckedOutCartsAsync(userName, cancellationToken);
        return carts
            .Select(cart => new UserOrderHistoryDto(cart.TotalPrice, cart.Created.ToString("d")))
            .OrderBy(history => history.Date)
            .Take(RecentOrderLimit)
            .ToList();
    }
}
