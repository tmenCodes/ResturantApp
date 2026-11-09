using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Cart.RemoveCartItem;

/// <summary>Removes a line from a cart.</summary>
public class RemoveCartItemHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCartItemHandler(IShoppingCartRepository shoppingCartRepository, IUnitOfWork unitOfWork)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Removes an order item from the cart named by <paramref name="cartIdentity"/> and recalculates its totals.
    /// Returns false when the cart does not exist or does not contain the order item.
    /// </summary>
    public async Task<bool> HandleAsync(int orderItemId, Guid cartIdentity, CancellationToken cancellationToken = default)
    {
        var cart = await _shoppingCartRepository.GetCartAsync(cartIdentity);
        if (cart is null || !cart.RemoveItem(orderItemId))
        {
            return false;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
