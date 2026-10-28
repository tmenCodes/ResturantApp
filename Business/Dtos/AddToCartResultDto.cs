namespace Resturant.Business.Dtos;

/// <summary>
/// Returned after adding an item to a cart. <see cref="Price"/> is the menu item's base price when the item started a
/// new cart, and the line's total price when it was added to an existing cart.
/// </summary>
public class AddToCartResultDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
