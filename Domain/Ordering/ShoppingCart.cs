namespace Resturant.Domain.Ordering;

/// <summary>A customer's cart. Keeps its subtotal, tax and total in step with its order items.</summary>
public class ShoppingCart
{
    /// <summary>The sales tax rate applied to new carts.</summary>
    public const decimal DefaultTaxRate = 0.06875m;

    public int Id { get; set; }
    public Guid Identity { get; set; }
    public DateTime Created { get; set; }

    public decimal SubTotal { get; private set; }

    public decimal TaxRate { get; set; } = DefaultTaxRate;

    public decimal TaxAmount { get; private set; }

    public decimal TotalPrice { get; private set; }

    public List<OrderItem> OrderItems { get; set; } = new();

    public int? CheckoutId { get; set; }

    public Checkout? Checkout { get; set; }

    /// <summary>Starts an empty cart with the default tax rate.</summary>
    public static ShoppingCart Create(Guid identity, DateTime createdUtc) =>
        new() { Identity = identity, Created = createdUtc };

    /// <summary>Adds an order item and recalculates the totals.</summary>
    public void AddItem(OrderItem item)
    {
        OrderItems.Add(item);
        RecalculateTotals();
    }

    /// <summary>Removes the order item with this id and recalculates the totals. Returns false when the cart has no such item.</summary>
    public bool RemoveItem(int orderItemId)
    {
        var item = OrderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item is null)
        {
            return false;
        }

        OrderItems.Remove(item);
        RecalculateTotals();
        return true;
    }

    private void RecalculateTotals()
    {
        SubTotal = OrderItems.Sum(x => x.TotalItemPrice);
        TaxAmount = SubTotal * TaxRate;
        TotalPrice = SubTotal + TaxAmount;
    }
}
