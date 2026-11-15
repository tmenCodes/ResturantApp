using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Menu.GetMenuItems;

/// <summary>Lists the menu.</summary>
public class GetMenuItemsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetMenuItemsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns every menu item with its base price formatted to two decimals.</summary>
    public async Task<IReadOnlyList<MenuItemDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var menuItems = await _context.MenuItems.AsNoTracking().ToListAsync(cancellationToken);
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
