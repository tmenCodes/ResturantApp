using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;

namespace Resturant.Domain.Services.Ordering;

/// <summary>An order item together with the menu item it was built from.</summary>
public record OrderItemFromMenu(MenuItem MenuItem, OrderItem OrderItem);
