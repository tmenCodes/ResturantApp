using Resturant.Application.Interfaces;
using Resturant.Application.Services;
using Resturant.Infrastructure.Persistence;
using Resturant.Domain.Menu;
using Resturant.Infrastructure.Persistence.Repositories;
namespace Application.Tests;

public class MenuServiceTests
{
    private static async Task SeedAsync(RestaurantContext context)
    {
        context.MenuItems.AddRange(
            new MenuItem("Papaya Salad", "Green papaya", 8.5m, "PapayaSalad.png"),
            new MenuItem("Tofu Stir Fry", "Tofu, peppers", 10m, "TofuStirFry.png"));
        context.MenuItemOptions.Add(new MenuItemOption("Extra Tofu", 1m));
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetMenuItemsAsync_MapsFieldsAndFormatsPriceWithTwoDecimals()
    {
        await using var context = TestDatabase.Create();
        await SeedAsync(context);

        var items = await new MenuService(new MenuRepository(context)).GetMenuItemsAsync();

        Assert.Equal(2, items.Count);
        var salad = Assert.Single(items, x => x.MenuItemName == "Papaya Salad");
        Assert.Equal(8.5m.ToString("0.00"), salad.MenuItemBasePrice);
        Assert.Equal("Green papaya", salad.MenuItemDescription);
        Assert.Equal("PapayaSalad.png", salad.MenuItemImageUrl);
    }

    [Fact]
    public async Task GetMenuItemOptionsAsync_ReturnsEveryOption()
    {
        await using var context = TestDatabase.Create();
        await SeedAsync(context);

        var options = await new MenuService(new MenuRepository(context)).GetMenuItemOptionsAsync();

        var option = Assert.Single(options);
        Assert.Equal("Extra Tofu", option.Name);
        Assert.Equal(1m, option.Price);
    }

    [Fact]
    public async Task GetImageUrlsAsync_ReturnsEveryMenuItemImage()
    {
        await using var context = TestDatabase.Create();
        await SeedAsync(context);

        var urls = await new MenuService(new MenuRepository(context)).GetImageUrlsAsync();

        Assert.Equal(new[] { "PapayaSalad.png", "TofuStirFry.png" }, urls.OrderBy(x => x));
    }
}
