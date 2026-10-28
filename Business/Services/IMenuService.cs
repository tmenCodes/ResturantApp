using Resturant.Business.Dtos;

namespace Resturant.Business.Services;

/// <summary>Provides the restaurant menu to the presentation layer.</summary>
public interface IMenuService
{
    /// <summary>Returns every menu item with its base price formatted to two decimals.</summary>
    Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns every add-on option.</summary>
    Task<IReadOnlyList<MenuItemOptionDto>> GetMenuItemOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns the image file name of every menu item.</summary>
    Task<IReadOnlyList<string>> GetImageUrlsAsync(CancellationToken cancellationToken = default);
}
