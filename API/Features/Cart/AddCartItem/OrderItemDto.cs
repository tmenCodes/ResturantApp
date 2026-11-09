using System.ComponentModel.DataAnnotations;

namespace API.Features.Cart.AddCartItem;

/// <summary>A menu item, with quantity and option names, to add to the cart named by <see cref="GuidId"/>.</summary>
public class OrderItemDto
{
    public Guid GuidId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }

    [Required]
    public List<string> Options { get; set; } = new();
}
