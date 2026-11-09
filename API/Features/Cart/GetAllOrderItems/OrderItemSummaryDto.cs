namespace API.Features.Cart.GetAllOrderItems;

/// <summary>An order item's id, name and line total. <see cref="GuidId"/> is never set and is always sent as null.</summary>
public class OrderItemSummaryDto
{
    public int Id { get; set; }
    public string? GuidId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}
