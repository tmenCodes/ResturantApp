using API.Common;
using API.Features.Cart.Shared;
using Resturant.Domain.Repositories;

namespace API.Features.Cart.GetCartItemCount;

/// <summary>Counts the lines in a cart.</summary>
public class GetCartItemCountHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public GetCartItemCountHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    /// <summary>Returns how many items are in a cart. A malformed or unknown identity yields zero.</summary>
    public async Task<int> HandleAsync(string? cartIdentity)
    {
        var orderItems = await _shoppingCartRepository.GetCartOrderItemsAsync(CartIdentity.ParseOrEmpty(cartIdentity));
        return orderItems.Count;
    }
}
