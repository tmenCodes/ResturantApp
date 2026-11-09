using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Menu.GetMenuItems;

/// <summary>Lists the menu.</summary>
public class GetMenuItemsHandler : IHandler
{
    private readonly IMenuRepository _menuRepository;

    public GetMenuItemsHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>Returns every menu item with its base price formatted to two decimals.</summary>
    public async Task<IReadOnlyList<MenuItemDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var menuItems = await _menuRepository.GetMenuItemsAsync(cancellationToken);
        return menuItems.Select(x => new MenuItemDto
        {
            Id = x.Id,
            MenuItemName = x.Name,
            MenuItemBasePrice = x.BasePrice.ToString("0.00"),
            MenuItemDescription = x.Description,
            MenuItemImageUrl = x.ImageUrl
        }).ToList();
    }
}
