using API.Features.Menu.GetMenuItems;

namespace API.Tests.Features.Menu;

public class GetMenuItemsHandlerTests
{
    [Fact]
    public async Task HandleAsync_MapsFieldsAndFormatsPriceWithTwoDecimals()
    {
        await using var context = TestDatabase.Create();
        await MenuTestData.SeedAsync(context);

        var items = await new GetMenuItemsHandler(context).HandleAsync();

        Assert.Equal(2, items.Count);
        var salad = Assert.Single(items, x => x.MenuItemName == "Papaya Salad");
        Assert.Equal(8.5m.ToString("0.00"), salad.MenuItemBasePrice);
        Assert.Equal("Green papaya", salad.MenuItemDescription);
        Assert.Equal("PapayaSalad.png", salad.MenuItemImageUrl);
    }
}
