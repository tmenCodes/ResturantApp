using Resturant.Domain.Exceptions;
using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;

namespace Resturant.Domain.Services.Ordering;

/// <summary>Default <see cref="IOrderItemFactory"/>, reading the menu through <see cref="IMenuRepository"/>.</summary>
public class OrderItemFactory : IOrderItemFactory
{
    private readonly IMenuRepository _menuRepository;

    public OrderItemFactory(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<OrderItemFromMenu> CreateAsync(string? menuItemName, IEnumerable<string> optionNames, int quantity, CancellationToken cancellationToken = default)
    {
        var menuItem = await _menuRepository.GetMenuItemByNameAsync(menuItemName, cancellationToken)
                       ?? throw new MenuItemNotFoundException(menuItemName);
        var options = await FindOptionsAsync(optionNames, cancellationToken);
        return new OrderItemFromMenu(menuItem, OrderItem.Create(menuItem, options, quantity));
    }

    private async Task<List<MenuItemOption>> FindOptionsAsync(IEnumerable<string> optionNames, CancellationToken cancellationToken)
    {
        var options = new List<MenuItemOption>();
        foreach (var optionName in optionNames)
        {
            var option = await _menuRepository.GetOptionByNameAsync(optionName, cancellationToken);
            if (option is not null)
            {
                options.Add(option);
            }
        }
        return options;
    }
}
