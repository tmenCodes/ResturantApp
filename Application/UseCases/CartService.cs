using Resturant.Application.Dtos;
using Resturant.Application.Mappings;
using Resturant.Application.Ports.In;
using Resturant.Domain.Repositories;
using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;

namespace Resturant.Application.UseCases;

/// <summary>Default <see cref="ICartService"/>. Pricing and totals are computed by the domain.</summary>
public class CartService : ICartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly TimeProvider _timeProvider;

    public CartService(IShoppingCartRepository shoppingCartRepository, IMenuRepository menuRepository, TimeProvider timeProvider)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _menuRepository = menuRepository;
        _timeProvider = timeProvider;
    }

    public async Task<ShoppingCartItemDTO?> GetTotalsAsync(string cartIdentity)
    {
        var cart = await _shoppingCartRepository.GetCartAsync(cartIdentity);
        if (cart is null)
        {
            return null;
        }

        return new ShoppingCartItemDTO
        {
            SubTotal = cart.SubTotal.ToString("0.00"),
            TaxAmount = cart.TaxAmount.ToString("0.00"),
            TotalPrice = cart.TotalPrice.ToString("0.00")
        };
    }

    public async Task<IReadOnlyList<MenuDTO>> GetAllOrderItemsAsync()
    {
        var orderItems = await _shoppingCartRepository.GetAllOrderItemsAsync();
        return orderItems.Select(x => new MenuDTO { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice }).ToList();
    }

    public async Task<IReadOnlyList<CartOrderItemDto>> GetCartItemsAsync(string cartIdentity)
    {
        Guid.TryParse(cartIdentity, out var identity);
        var orderItems = await _shoppingCartRepository.GetCartOrderItemsAsync(identity.ToString());
        return orderItems.Select(x => x.ToCartDto()).ToList();
    }

    public async Task<int> GetCartItemCountAsync(string cartIdentity)
    {
        return (await GetCartItemsAsync(cartIdentity)).Count;
    }

    public async Task<bool> RemoveOrderItemAsync(int orderItemId, Guid cartIdentity)
    {
        var cart = await _shoppingCartRepository.GetCartAsync(cartIdentity.ToString());
        if (cart is null || !cart.RemoveItem(orderItemId))
        {
            return false;
        }

        await _shoppingCartRepository.SaveChangesAsync();
        return true;
    }

    public async Task<AddToCartResultDto> AddItemAsync(OrderItemDto request, CancellationToken cancellationToken = default)
    {
        var menuItem = await _menuRepository.GetMenuItemByNameAsync(request.Name, cancellationToken)
                       ?? throw new InvalidOperationException($"Menu item '{request.Name}' was not found.");
        var options = await FindOptionsAsync(request.Options, cancellationToken);
        var orderItem = OrderItem.Create(menuItem, options, request.Quantity);

        var cart = await _shoppingCartRepository.GetCartAsync(request.GuidId.ToString());
        var isNewCart = cart is null;
        cart ??= ShoppingCart.Create(request.GuidId, _timeProvider.GetUtcNow().UtcDateTime);
        cart.AddItem(orderItem);

        if (isNewCart)
        {
            await _shoppingCartRepository.AddCartAsync(cart);
        }
        await _shoppingCartRepository.SaveChangesAsync();

        var price = isNewCart ? menuItem.BasePrice : orderItem.TotalItemPrice;
        return new AddToCartResultDto { Name = menuItem.Name, Price = price };
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
