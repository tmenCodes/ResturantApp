using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.GetAllOrderItems;

/// <summary>Lists the order items in every cart.</summary>
public class GetAllOrderItemsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetAllOrderItemsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns every order item across all carts.</summary>
    public async Task<IReadOnlyList<OrderItemSummaryDto>> HandleAsync()
    {
        return await _context.ShoppingCarts
            .SelectMany(x => x.OrderItems)
            .Select(x => new OrderItemSummaryDto { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice })
            .ToListAsync();
    }
}
