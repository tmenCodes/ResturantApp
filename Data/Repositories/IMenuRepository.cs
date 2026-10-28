using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>Reads the restaurant menu: menu items and the add-on options customers can choose.</summary>
public interface IMenuRepository
{
    /// <summary>Returns every menu item, in database order.</summary>
    Task<List<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the menu item with exactly this name, or null.</summary>
    Task<MenuItem?> GetMenuItemByNameAsync(string? name, CancellationToken cancellationToken = default);

    /// <summary>Returns every menu option, in database order.</summary>
    Task<List<MenuItemOption>> GetOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the menu option with exactly this name, or null.</summary>
    Task<MenuItemOption?> GetOptionByNameAsync(string? name, CancellationToken cancellationToken = default);
}
