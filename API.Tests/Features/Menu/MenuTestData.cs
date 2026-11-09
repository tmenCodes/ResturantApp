using Resturant.Domain.Menu;
using Resturant.Infrastructure.Persistence;

namespace API.Tests.Features.Menu;

/// <summary>Seeds a small menu for the menu handler tests.</summary>
public static class MenuTestData
{
    public static async Task SeedAsync(RestaurantContext context)
    {
        context.MenuItems.AddRange(
            new MenuItem("Papaya Salad", "Green papaya", 8.5m, "PapayaSalad.png"),
            new MenuItem("Tofu Stir Fry", "Tofu, peppers", 10m, "TofuStirFry.png"));
        context.MenuItemOptions.Add(new MenuItemOption("Extra Tofu", 1m));
        await context.SaveChangesAsync();
    }
}
