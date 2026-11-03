using Resturant.Application.Dtos;
using Resturant.Application.Mappings;
using Resturant.Application.Interfaces;
using Resturant.Domain.Repositories;
using Resturant.Domain.Ordering;
using Resturant.Domain.Services.Ordering;

namespace Resturant.Application.Services;

/// <summary>Default <see cref="ICartService"/>. Order items are built by <see cref="IOrderItemFactory"/>; totals by <see cref="ShoppingCart"/>.</summary>
public class CartService : ICartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IOrderItemFactory _orderItemFactory;
    private readonly TimeProvider _timeProvider;

    public CartService(IShoppingCartRepository shoppingCartRepository, IOrderItemFactory orderItemFactory, TimeProvider timeProvider)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _orderItemFactory = orderItemFactory;
        _timeProvider = timeProvider;
    }

    public async Task<ShoppingCartItemDTO?> GetTotalsAsync(string cartIdentity)
    {
        if (!Guid.TryParse(cartIdentity, out var identity))
        {
            return null;
        }

        var cart = await _shoppingCartRepository.GetCartAsync(identity);
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
        var orderItems = await _shoppingCartRepository.GetCartOrderItemsAsync(identity);
        return orderItems.Select(x => x.ToCartDto()).ToList();
    }

    public async Task<int> GetCartItemCountAsync(string cartIdentity)
    {
        return (await GetCartItemsAsync(cartIdentity)).Count;
    }

    public async Task<bool> RemoveOrderItemAsync(int orderItemId, Guid cartIdentity)
    {
        var cart = await _shoppingCartRepository.GetCartAsync(cartIdentity);
        if (cart is null || !cart.RemoveItem(orderItemId))
        {
            return false;
        }

        await _shoppingCartRepository.SaveChangesAsync();
        return true;
    }

    public async Task<AddToCartResultDto> AddItemAsync(OrderItemDto request, CancellationToken cancellationToken = default)
    {
        var (menuItem, orderItem) = await _orderItemFactory.CreateAsync(request.Name, request.Options, request.Quantity, cancellationToken);

        var cart = await _shoppingCartRepository.GetCartAsync(request.GuidId);
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
}
