using Resturant.Domain.Menu;
using Resturant.Domain.Cart;
using Resturant.Domain.Ordering;
using Resturant.Domain.Payments;

namespace Resturant.Domain.Services.Ordering;

/// <summary>An order item together with the menu item it was built from.</summary>
public record OrderItemFromMenu(MenuItem MenuItem, OrderItem OrderItem);
