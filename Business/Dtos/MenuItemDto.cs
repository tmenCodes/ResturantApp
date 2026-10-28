namespace Resturant.Business.Dtos;

/// <summary>A menu item as shown on the menu page. The price is preformatted with two decimals.</summary>
public class MenuItemDto
{
    public int Id { get; set; }
    public string MenuItemName { get; set; } = string.Empty;
    public string MenuItemBasePrice { get; set; } = string.Empty;
    public string MenuItemDescription { get; set; } = string.Empty;
    public string MenuItemImageUrl { get; set; } = string.Empty;
}
