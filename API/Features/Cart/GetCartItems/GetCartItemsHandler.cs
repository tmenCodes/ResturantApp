using API.Common;
using API.Features.Cart.Shared;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.GetCartItems;

/// <summary>Lists the lines in a cart.</summary>
public class GetCartItemsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetCartItemsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns the items in a cart with their options. A malformed or unknown identity yields an empty list.</summary>
    public async Task<IReadOnlyList<CartOrderItemDto>> HandleAsync(string? cartIdentity)
    {
        var identity = CartIdentity.ParseOrEmpty(cartIdentity);
        return await _context.ShoppingCarts
            .Where(x => x.Identity == identity)
            .SelectMany(x => x.OrderItems)
            .Select(orderItem => new CartOrderItemDto
            {
                Id = orderItem.Id,
                Name = orderItem.Name,
                Quantity = orderItem.Quantity,
                TotalItemPrice = orderItem.TotalItemPrice,
                Status = orderItem.Status,
                ShoppingCartIdentity = orderItem.ShoppingCartIdentity,
                Options = orderItem.Options.Select(x => new CartOrderItemOptionDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    OrderItemId = x.OrderItemId
                }).ToList()
            })
            .ToListAsync();
    }
}
