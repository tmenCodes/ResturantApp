namespace Resturant.Domain.Services.Ordering;

/// <summary>Builds order items from the restaurant menu.</summary>
public interface IOrderItemFactory
{
    /// <summary>
    /// Builds an order item for <paramref name="quantity"/> of the menu item named <paramref name="menuItemName"/>
    /// with the named options. Option names that are not on the menu are ignored.
    /// </summary>
    /// <exception cref="InvalidOperationException">No menu item has this name.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The quantity is not greater than zero.</exception>
    Task<OrderItemFromMenu> CreateAsync(string? menuItemName, IEnumerable<string> optionNames, int quantity, CancellationToken cancellationToken = default);
}
