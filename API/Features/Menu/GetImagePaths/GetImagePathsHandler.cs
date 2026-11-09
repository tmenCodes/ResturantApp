using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Menu.GetImagePaths;

/// <summary>Lists the menu's image file names.</summary>
public class GetImagePathsHandler : IHandler
{
    private readonly IMenuRepository _menuRepository;

    public GetImagePathsHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>Returns the image file name of every menu item.</summary>
    public async Task<IReadOnlyList<string>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var menuItems = await _menuRepository.GetMenuItemsAsync(cancellationToken);
        return menuItems.Select(x => x.ImageUrl).ToList();
    }
}
