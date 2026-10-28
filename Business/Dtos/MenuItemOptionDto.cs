namespace Resturant.Business.Dtos;

/// <summary>An add-on a customer can choose for a menu item, such as extra chicken.</summary>
public class MenuItemOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
