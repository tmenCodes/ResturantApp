using Microsoft.EntityFrameworkCore;
using Resturant.Data.Context;
using Resturant.Data.Entities;

namespace Resturant.Data.Repositories;

/// <summary>EF Core implementation of <see cref="IMenuRepository"/>.</summary>
public class MenuRepository : IMenuRepository
{
    private readonly RestaurantContext _context;

    public MenuRepository(RestaurantContext context)
    {
        _context = context;
    }

    public Task<List<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default)
    {
        return _context.MenuItems.ToListAsync(cancellationToken);
    }

    public Task<MenuItem?> GetMenuItemByNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        return _context.MenuItems.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }

    public Task<List<MenuItemOption>> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        return _context.MenuItemOptions.ToListAsync(cancellationToken);
    }

    public Task<MenuItemOption?> GetOptionByNameAsync(string? name, CancellationToken cancellationToken = default)
    {
        return _context.MenuItemOptions.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
    }
}
