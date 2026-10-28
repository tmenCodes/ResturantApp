using Resturant.Business.Dtos;
using Resturant.Data.Repositories;

namespace Resturant.Business.Services;

/// <summary>Default <see cref="IMenuService"/> backed by <see cref="IMenuRepository"/>.</summary>
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
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

    public async Task<IReadOnlyList<MenuItemOptionDto>> GetMenuItemOptionsAsync(CancellationToken cancellationToken = default)
    {
        var options = await _menuRepository.GetOptionsAsync(cancellationToken);
        return options.Select(x => new MenuItemOptionDto { Id = x.Id, Name = x.Name, Price = x.Price }).ToList();
    }

    public async Task<IReadOnlyList<string>> GetImageUrlsAsync(CancellationToken cancellationToken = default)
    {
        var menuItems = await _menuRepository.GetMenuItemsAsync(cancellationToken);
        return menuItems.Select(x => x.ImageUrl).ToList();
    }
}
