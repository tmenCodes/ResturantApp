using Resturant.Data.Entities;

namespace API.Contracts;

/// <summary>An order with the customer name and the menu items it contains.</summary>
public class ViewsDto
{
    public required string Name { get; set; }
    public Guid OrderId { get; set; }
    public required List<MenuItem> Menus { get; set; }
}
