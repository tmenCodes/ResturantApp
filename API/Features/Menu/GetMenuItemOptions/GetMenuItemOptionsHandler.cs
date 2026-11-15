using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Menu.GetMenuItemOptions;

/// <summary>Lists the add-on options.</summary>
public class GetMenuItemOptionsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetMenuItemOptionsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns every add-on option.</summary>
    public async Task<IReadOnlyList<MenuItemOptionDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuItemOptions
            .Select(x => new MenuItemOptionDto { Id = x.Id, Name = x.Name, Price = x.Price })
            .ToListAsync(cancellationToken);
    }
}
