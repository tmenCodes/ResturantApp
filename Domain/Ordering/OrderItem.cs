using Resturant.Domain.Menu;

namespace Resturant.Domain.Ordering;

/// <summary>A menu item in a cart, with the options chosen for it and its price for the ordered quantity.</summary>
public class OrderItem
{
    /// <summary>The status every new order item starts with.</summary>
    public const string InitialStatus = "start";

    private readonly List<OrderItemOption> _options = new();

    private OrderItem()
    {
        Name = string.Empty;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalItemPrice { get; private set; }
    public string? Status { get; private set; }

    public IReadOnlyCollection<OrderItemOption> Options => _options;
    public Guid ShoppingCartIdentity { get; private set; }
    public ShoppingCart? ShoppingCart { get; private set; }

    /// <summary>
    /// Creates an order item for <paramref name="quantity"/> of <paramref name="menuItem"/> with the chosen options.
    /// Its price is the base price plus every option's price, times the quantity.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The quantity is not greater than zero.</exception>
    public static OrderItem Create(MenuItem menuItem, IEnumerable<MenuItemOption> options, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        var item = new OrderItem { Name = menuItem.Name, Status = InitialStatus, Quantity = quantity };
        item._options.AddRange(options.Select(option => new OrderItemOption(option.Name, option.Price)));
        item.TotalItemPrice = (menuItem.BasePrice + item._options.Sum(x => x.Price)) * quantity;
        return item;
    }
}
