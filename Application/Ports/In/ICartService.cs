using Resturant.Application.Dtos;
namespace Resturant.Application.Ports.In;

/// <summary>Manages shopping carts: adding and removing items and keeping subtotal, tax and total up to date.</summary>
public interface ICartService
{
    /// <summary>Returns the cart's subtotal, tax and total formatted to two decimals, or null when the cart does not exist.</summary>
    Task<ShoppingCartItemDTO?> GetTotalsAsync(string cartIdentity);

    /// <summary>Returns every order item across all carts.</summary>
    Task<IReadOnlyList<MenuDTO>> GetAllOrderItemsAsync();

    /// <summary>Returns the items in a cart with their options. A malformed or unknown identity yields an empty list.</summary>
    Task<IReadOnlyList<CartOrderItemDto>> GetCartItemsAsync(string cartIdentity);

    /// <summary>Returns how many items are in a cart.</summary>
    Task<int> GetCartItemCountAsync(string cartIdentity);

    /// <summary>
    /// Removes an order item from the cart named by <paramref name="cartIdentity"/> and recalculates its totals.
    /// Returns false when the cart does not exist or does not contain the order item.
    /// </summary>
    Task<bool> RemoveOrderItemAsync(int orderItemId, Guid cartIdentity);

    /// <summary>
    /// Adds a menu item, with its options, to the cart named by <see cref="OrderItemDto.GuidId"/>, creating the cart
    /// when it does not exist yet. Unknown option names are ignored.
    /// </summary>
    /// <exception cref="InvalidOperationException">No menu item has the requested name.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The quantity is not greater than zero.</exception>
    Task<AddToCartResultDto> AddItemAsync(OrderItemDto request, CancellationToken cancellationToken = default);
}
