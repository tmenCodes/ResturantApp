using API.Common;
using Microsoft.EntityFrameworkCore;
using Resturant.Domain.Cart;
using Resturant.Infrastructure.Persistence;

namespace API.Features.Cart.AddCartItem;

/// <summary>Adds a menu item to a cart. Order items are built by <see cref="MenuOrderItemBuilder"/>; totals by <see cref="ShoppingCart"/>.</summary>
public class AddCartItemHandler : IHandler
{
    private readonly RestaurantContext _context;
    private readonly TimeProvider _timeProvider;

    public AddCartItemHandler(RestaurantContext context, TimeProvider timeProvider)
    {
        _context = context;
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
        var (menuItem, orderItem) = await MenuOrderItemBuilder.BuildAsync(_context, request.Name, request.Options, request.Quantity, cancellationToken);

        var cart = await _context.ShoppingCarts
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.Identity == request.GuidId, cancellationToken);
        var isNewCart = cart is null;
        cart ??= ShoppingCart.Create(request.GuidId, _timeProvider.GetUtcNow().UtcDateTime);
        cart.AddItem(orderItem);

        if (isNewCart)
        {
            await _context.ShoppingCarts.AddAsync(cart, cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);

        var price = isNewCart ? menuItem.BasePrice : orderItem.TotalItemPrice;
        return new AddToCartResultDto { Name = menuItem.Name, Price = price };
    }
}
