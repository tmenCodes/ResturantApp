namespace Resturant.Domain.Ordering;

/// <summary>An option chosen for an order item, with the price it had when it was chosen.</summary>
public class OrderItemOption
{
    private OrderItemOption()
    {
        Name = string.Empty;
    }

    internal OrderItemOption(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public OrderItem OrderItem { get; private set; } = null!;
    public int OrderItemId { get; private set; }
}
