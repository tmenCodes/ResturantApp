using API.Common;
using API.Features.Cart.Shared;
using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using Resturant.Domain.Repositories;

namespace API.Features.Cart.GetCartItems;

/// <summary>Lists the lines in a cart.</summary>
public class GetCartItemsHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public GetCartItemsHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    /// <summary>Returns the items in a cart with their options. A malformed or unknown identity yields an empty list.</summary>
    public async Task<IReadOnlyList<CartOrderItemDto>> HandleAsync(string? cartIdentity)
    {
        var orderItems = await _shoppingCartRepository.GetCartOrderItemsAsync(CartIdentity.ParseOrEmpty(cartIdentity));
        return orderItems.Select(ToDto).ToList();
    }

    private static CartOrderItemDto ToDto(OrderItem orderItem) => new()
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
    };
}
