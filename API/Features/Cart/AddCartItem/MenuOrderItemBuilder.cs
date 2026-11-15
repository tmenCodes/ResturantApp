using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Exceptions;
using Resturant.Domain.Menu;
using Resturant.Domain.Cart;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.AddCartItem;

/// <summary>Builds an <see cref="OrderItem"/> from the menu stored in <see cref="RestaurantContext"/>.</summary>
internal static class MenuOrderItemBuilder
{
    /// <summary>
    /// Builds an order item for <paramref name="quantity"/> of the menu item named <paramref name="menuItemName"/>
    /// with the named options, together with the menu item it was built from. Option names that are not on the menu
    /// are ignored.
    /// </summary>
    /// <exception cref="MenuItemNotFoundException">No menu item has this name.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The quantity is not greater than zero.</exception>
    public static async Task<(MenuItem MenuItem, OrderItem OrderItem)> BuildAsync(
        RestaurantContext context, string? menuItemName, IEnumerable<string> optionNames, int quantity, CancellationToken cancellationToken)
    {
        var menuItem = await context.MenuItems.FirstOrDefaultAsync(x => x.Name == menuItemName, cancellationToken)
                       ?? throw new MenuItemNotFoundException(menuItemName);
        var options = await FindOptionsAsync(context, optionNames, cancellationToken);
        return (menuItem, OrderItem.Create(menuItem, options, quantity));
    }

    private static async Task<List<MenuItemOption>> FindOptionsAsync(
        RestaurantContext context, IEnumerable<string> optionNames, CancellationToken cancellationToken)
    {
        var options = new List<MenuItemOption>();
        foreach (var optionName in optionNames)
        {
            var option = await context.MenuItemOptions.FirstOrDefaultAsync(x => x.Name == optionName, cancellationToken);
            if (option is not null)
            {
                options.Add(option);
            }
        }
        return options;
    }
}
