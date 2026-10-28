namespace Resturant.Business.Dtos;

/// <summary>A line in a shopping cart: one menu item, its quantity, chosen options and total price.</summary>
public class CartOrderItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalItemPrice { get; set; }
    public string? Status { get; set; }
    public List<CartOrderItemOptionDto> Options { get; set; } = new();
    public Guid ShoppingCartIdentity { get; set; }
}

/// <summary>An option chosen for a cart line, with the price it had when added.</summary>
public class CartOrderItemOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int OrderItemId { get; set; }
}
