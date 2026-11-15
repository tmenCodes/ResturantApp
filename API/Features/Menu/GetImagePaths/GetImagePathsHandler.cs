using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Menu.GetImagePaths;

/// <summary>Lists the menu's image file names.</summary>
public class GetImagePathsHandler : IHandler
{
    private readonly RestaurantContext _context;

    public GetImagePathsHandler(RestaurantContext context)
    {
        _context = context;
    }

    /// <summary>Returns the image file name of every menu item.</summary>
    public async Task<IReadOnlyList<string>> HandleAsync(CancellationToken cancellationToken = default)
    {
        return await _context.MenuItems.Select(x => x.ImageUrl).ToListAsync(cancellationToken);
    }
}
