namespace Resturant.Domain.Menu;

/// <summary>An add-on a customer can choose for a menu item, with its extra price.</summary>
public class MenuItemOption
{
    private MenuItemOption()
    {
        Name = string.Empty;
    }

    public MenuItemOption(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
}
