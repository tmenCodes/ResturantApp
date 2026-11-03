namespace Resturant.Domain.Exceptions;

/// <summary>Raised when no menu item has the requested name.</summary>
public class MenuItemNotFoundException : NotFoundException
{
    public MenuItemNotFoundException(string? menuItemName)
        : base($"Menu item '{menuItemName}' was not found.")
    {
        MenuItemName = menuItemName;
    }

    /// <summary>The name that was looked up.</summary>
    public string? MenuItemName { get; }
}
