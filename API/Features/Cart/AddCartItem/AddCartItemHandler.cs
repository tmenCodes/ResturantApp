using API.Common;
using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;
using Resturant.Domain.Repositories;
using Resturant.Domain.Services.Ordering;

namespace API.Features.Cart.AddCartItem;

/// <summary>Adds a menu item to a cart. Order items are built by <see cref="IOrderItemFactory"/>; totals by <see cref="ShoppingCart"/>.</summary>
public class AddCartItemHandler : IHandler
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IOrderItemFactory _orderItemFactory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public AddCartItemHandler(IShoppingCartRepository shoppingCartRepository, IOrderItemFactory orderItemFactory, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _orderItemFactory = orderItemFactory;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Adds a menu item, with its options, to the cart named by <see cref="OrderItemDto.GuidId"/>, creating the cart
    /// when it does not exist yet. Unknown option names are ignored.
    /// </summary>
    /// <exception cref="Resturant.Domain.Exceptions.MenuItemNotFoundException">No menu item has the requested name.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The quantity is not greater than zero.</exception>
    public async Task<AddToCartResultDto> HandleAsync(OrderItemDto request, CancellationToken cancellationToken = default)
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var price = isNewCart ? menuItem.BasePrice : orderItem.TotalItemPrice;
        return new AddToCartResultDto { Name = menuItem.Name, Price = price };
    }
}
