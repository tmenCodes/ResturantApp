namespace Resturant.Domain.Ordering;

/// <summary>A customer's cart. Keeps its subtotal, tax and total in step with its order items.</summary>
public class ShoppingCart
{
    /// <summary>The sales tax rate applied to new carts.</summary>
    public const decimal DefaultTaxRate = 0.06875m;

    private readonly List<OrderItem> _orderItems = new();

    private ShoppingCart()
    {
    }

    public int Id { get; private set; }
    public Guid Identity { get; private set; }
    public DateTime Created { get; private set; }

    public decimal SubTotal { get; private set; }
    public decimal TaxRate { get; private set; } = DefaultTaxRate;
    public decimal TaxAmount { get; private set; }
    public decimal TotalPrice { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public int? CheckoutId { get; private set; }
    public Checkout? Checkout { get; private set; }

    /// <summary>Starts an empty cart with the default tax rate.</summary>
    public static ShoppingCart Create(Guid identity, DateTime createdUtc) =>
        new() { Identity = identity, Created = createdUtc };

    /// <summary>Adds an order item and recalculates the totals.</summary>
    public void AddItem(OrderItem item)
    {
        _orderItems.Add(item);
        RecalculateTotals();
    }

    /// <summary>Removes the order item with this id and recalculates the totals. Returns false when the cart has no such item.</summary>
    public bool RemoveItem(int orderItemId)
    {
        var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item is null)
        {
            return false;
        }

        _orderItems.Remove(item);
        RecalculateTotals();
        return true;
    }

    private void RecalculateTotals()
    {
        SubTotal = _orderItems.Sum(x => x.TotalItemPrice);
        TaxAmount = SubTotal * TaxRate;
        TotalPrice = SubTotal + TaxAmount;
    }
}
