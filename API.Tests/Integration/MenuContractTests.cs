using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class MenuContractTests : ContractTest
{
    [Fact]
    public async Task GetMenuItems_ReturnsSeededMenuWithFormattedPrices()
    {
        var response = await Client.GetSnapshotAsync("api/Order/GetMenuItems");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        Assert.Equal(6, response.JsonBody!.AsArray().Count);
        Assert.StartsWith(
            """[{"id":1,"menuItemName":"Egg Roll Platter","menuItemBasePrice":"14.95","menuItemDescription":"Carrots, Tomato, Lettuce","menuItemImageUrl":"EggRollPlatter.png"},""",
            response.Body);
    }

    [Fact]
    public async Task GetMenuItemOptions_ReturnsSeededOptions()
    {
        var response = await Client.GetSnapshotAsync("api/Order/GetMenuItemOptions");

        Assert.Equal(
            new HttpSnapshot(200, Json,
                """[{"id":1,"name":"Extra Chicken","price":1},{"id":2,"name":"Extra Beef","price":1},{"id":3,"name":"Extra Pork","price":1},{"id":4,"name":"Extra Tofu","price":1},{"id":5,"name":"Soy Sauce","price":0.25}]"""),
            response);
    }

    [Fact]
    public async Task ImagePath_ReturnsImageFileNames()
    {
        var response = await Client.GetSnapshotAsync("api/Order/ImagePath");

        Assert.Equal(
            new HttpSnapshot(200, Json,
                """["EggRollPlatter.png","PapayaSalad.png","TofuStirFry.png","ChoppedBeef.png","VeggiePlatter.png","CaesarSalad.png"]"""),
            response);
    }
}
