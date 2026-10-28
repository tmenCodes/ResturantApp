using Resturant.Business.Dtos;
using Resturant.Business.Mappings;
using Resturant.Data.Entities;
using Resturant.Data.Repositories;

namespace Resturant.Business.Services;

/// <summary>Default <see cref="ICartService"/>.</summary>
public class CartService : ICartService
{
    private const string NewOrderItemStatus = "start";

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
        var cart = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(cartIdentity);
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
        var orderItems = await _shoppingCartRepository.ReturnMenuItemsListAsync();
        return orderItems.Select(x => new MenuDTO { Id = x.Id, Name = x.Name, Price = x.TotalItemPrice }).ToList();
    }

    public async Task<IReadOnlyList<CartOrderItemDto>> GetCartItemsAsync(string cartIdentity)
    {
        Guid.TryParse(cartIdentity, out var identity);
        var orderItems = await _shoppingCartRepository.ReturnMenuItemListByGuid(identity.ToString());
        return orderItems.Select(x => x.ToCartDto()).ToList();
    }

    public async Task<int> GetCartItemCountAsync(string cartIdentity)
    {
        return (await GetCartItemsAsync(cartIdentity)).Count;
    }

    public async Task<bool> RemoveOrderItemAsync(int orderItemId, Guid cartIdentity)
    {
        var orderItem = await _shoppingCartRepository.FindByPrimaryKey(orderItemId);
        if (orderItem is null)
        {
            return false;
        }

        _shoppingCartRepository.RemoveOrderItem(orderItem);
        await _shoppingCartRepository.SaveCartItemsAsync();

        var cart = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(cartIdentity.ToString())
                   ?? throw new InvalidOperationException($"Shopping cart {cartIdentity} was not found.");
        ApplyTotals(cart, _shoppingCartRepository.SubTotalMenuPrice(cartIdentity));
        await _shoppingCartRepository.SaveCartItemsAsync();
        return true;
    }

    public async Task<AddToCartResultDto> AddItemAsync(OrderItemDto request, CancellationToken cancellationToken = default)
    {
        var cart = await _shoppingCartRepository.ReturnCartItemsByGuidAsync(request.GuidId.ToString());
        var (orderItem, menuItem) = await BuildOrderItemAsync(request, cancellationToken);

        if (cart is null)
        {
            await CreateCartAsync(request.GuidId, orderItem);
            return new AddToCartResultDto { Name = menuItem.Name, Price = menuItem.BasePrice };
        }

        var existingSubTotal = _shoppingCartRepository.SubTotalMenuPrice(request.GuidId);
        cart.OrderItems.Add(orderItem);
        ApplyTotals(cart, existingSubTotal + orderItem.TotalItemPrice);
        await _shoppingCartRepository.SaveCartItemsAsync();
        return new AddToCartResultDto { Name = menuItem.Name, Price = orderItem.TotalItemPrice };
    }

    private async Task<(OrderItem OrderItem, MenuItem MenuItem)> BuildOrderItemAsync(OrderItemDto request, CancellationToken cancellationToken)
    {
        var menuItem = await _menuRepository.GetMenuItemByNameAsync(request.Name, cancellationToken)
                       ?? throw new InvalidOperationException($"Menu item '{request.Name}' was not found.");
        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "value must be greater than 0");
        }

        var orderItem = new OrderItem { Name = menuItem.Name, Status = NewOrderItemStatus, Quantity = request.Quantity };
        foreach (var optionName in request.Options)
        {
            var option = await _menuRepository.GetOptionByNameAsync(optionName, cancellationToken);
            if (option is not null)
            {
                orderItem.Options.Add(new OrderItemOption { Name = option.Name, Price = option.Price });
            }
        }

        orderItem.TotalItemPrice = (menuItem.BasePrice + orderItem.Options.Sum(x => x.Price)) * request.Quantity;
        return (orderItem, menuItem);
    }

    private async Task CreateCartAsync(Guid identity, OrderItem firstItem)
    {
        var cart = new ShoppingCart { Identity = identity, Created = _timeProvider.GetUtcNow().UtcDateTime };
        cart.OrderItems.Add(firstItem);
        ApplyTotals(cart, firstItem.TotalItemPrice);
        await _shoppingCartRepository.AddShoppingCartItem(cart);
        await _shoppingCartRepository.SaveCartItemsAsync();
    }

    private static void ApplyTotals(ShoppingCart cart, decimal subTotal)
    {
        cart.SubTotal = subTotal;
        cart.TaxAmount = subTotal * cart.TaxRate;
        cart.TotalPrice = subTotal + cart.TaxAmount;
    }
}
