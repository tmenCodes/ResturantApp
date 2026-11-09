using API.Common;
using Resturant.Domain.Repositories;

namespace API.Features.Menu.GetMenuItemOptions;

/// <summary>Lists the add-on options.</summary>
public class GetMenuItemOptionsHandler : IHandler
{
    private readonly IMenuRepository _menuRepository;

    public GetMenuItemOptionsHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    /// <summary>Returns every add-on option.</summary>
    public async Task<IReadOnlyList<MenuItemOptionDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var options = await _menuRepository.GetOptionsAsync(cancellationToken);
        return options.Select(x => new MenuItemOptionDto { Id = x.Id, Name = x.Name, Price = x.Price }).ToList();
    }
}
