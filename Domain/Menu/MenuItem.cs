namespace Resturant.Domain.Menu;

/// <summary>A dish on the menu with its base price before options.</summary>
public class MenuItem
{
    private MenuItem()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    public MenuItem(string name, string description, decimal basePrice, string imageUrl = "")
    {
        Name = name;
        Description = description;
        BasePrice = basePrice;
        ImageUrl = imageUrl;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public decimal BasePrice { get; private set; }
}
