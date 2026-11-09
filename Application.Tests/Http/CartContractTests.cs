using static Application.Tests.Http.HttpSnapshot;

namespace Application.Tests.Http;

public class CartContractTests : ContractTest
{
    private readonly Guid _cart = Guid.NewGuid();

    private async Task<int> FirstOrderItemIdAsync()
    {
        var items = await Client.GetSnapshotAsync($"api/Order/GetMenuItemByGuid?guidId={_cart}");
        return (int)items.JsonBody![0]!["id"]!;
    }

    [Fact]
    public async Task ShoppingCartItems_NewCart_ReturnsBasePrice()
    {
        var response = await Client.AddToCartAsync(_cart);

        Assert.Equal(new HttpSnapshot(200, Json, """{"name":"Tofu Stir Fry","price":10.95}"""), response);
    }

    [Fact]
    public async Task ShoppingCartItems_ExistingCart_ReturnsLineTotal()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.AddToCartAsync(_cart, quantity: 2, options: "Soy Sauce");

        Assert.Equal(new HttpSnapshot(200, Json, """{"name":"Tofu Stir Fry","price":22.40}"""), response);
    }

    [Fact]
    public async Task ShoppingCartItems_UnknownMenuItem_Returns404WithMessage()
    {
        var response = await Client.AddToCartAsync(_cart, name: "Pizza");

        Assert.Equal(new HttpSnapshot(404, Text, "Menu item 'Pizza' was not found."), response);
    }

    [Fact]
    public async Task GetTotalPrice_ReturnsFormattedTotals()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"api/Order/GetTotalPrice?guid={_cart}");

        Assert.Equal(new HttpSnapshot(200, Json, """{"subTotal":"10.95","taxAmount":"0.75","totalPrice":"11.70"}"""), response);
    }

    [Fact]
    public async Task GetTotalPrice_UnknownCart_Returns404Problem()
    {
        var response = await Client.GetSnapshotAsync($"api/Order/GetTotalPrice?guid={_cart}");

        response.AssertProblem(404);
    }

    [Fact]
    public async Task GetTotalPrice_MalformedGuid_Returns404Problem()
    {
        var response = await Client.GetSnapshotAsync("api/Order/GetTotalPrice?guid=not-a-guid");

        response.AssertProblem(404);
    }

    [Fact]
    public async Task GetAllTempItems_NoItems_Returns404WithMessage()
    {
        var response = await Client.GetSnapshotAsync("api/Order/GetAllTempItems");

        Assert.Equal(new HttpSnapshot(404, Text, "no value was found"), response);
    }

    [Fact]
    public async Task GetAllTempItems_ReturnsEveryOrderItem()
    {
        await Client.AddToCartAsync(_cart);
        var id = await FirstOrderItemIdAsync();

        var response = await Client.GetSnapshotAsync("api/Order/GetAllTempItems");

        Assert.Equal(new HttpSnapshot(200, Json, $$"""[{"id":{{id}},"guidId":null,"name":"Tofu Stir Fry","price":10.95}]"""), response);
    }

    [Fact]
    public async Task GetMenuItemByGuid_ReturnsCartLinesWithOptions()
    {
        await Client.AddToCartAsync(_cart, options: "Soy Sauce");

        var response = await Client.GetSnapshotAsync($"api/Order/GetMenuItemByGuid?guidId={_cart}");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        var line = response.JsonBody![0]!;
        var option = line["options"]![0]!;
        Assert.Equal(
            $$"""[{"id":{{line["id"]}},"name":"Tofu Stir Fry","quantity":1,"totalItemPrice":11.20,"status":"start","options":[{"id":{{option["id"]}},"name":"Soy Sauce","price":0.25,"orderItemId":{{line["id"]}}}],"shoppingCartIdentity":"{{_cart}}"}]""",
            response.Body);
    }

    [Fact]
    public async Task GetMenuItemByGuid_WithoutGuid_UsesDefaultAndReturnsEmptyList()
    {
        var response = await Client.GetSnapshotAsync("api/Order/GetMenuItemByGuid");

        Assert.Equal(new HttpSnapshot(200, Json, "[]"), response);
    }

    [Fact]
    public async Task GetMenuItemByGuidSize_ReturnsLineCount()
    {
        await Client.AddToCartAsync(_cart);
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"api/Order/GetMenuItemByGuidSize?guidId={_cart}");

        Assert.Equal(new HttpSnapshot(200, Json, "2"), response);
    }

    [Fact]
    public async Task DeleteMenuItem_RemovesLineAndReturnsEmpty200()
    {
        await Client.AddToCartAsync(_cart);
        var id = await FirstOrderItemIdAsync();

        var response = await Client.DeleteSnapshotAsync($"api/Order/DeleteMenuItem?id={id}&guidId={_cart}");

        Assert.Equal(new HttpSnapshot(200, null, ""), response);
        Assert.Equal("0", (await Client.GetSnapshotAsync($"api/Order/GetMenuItemByGuidSize?guidId={_cart}")).Body);
    }

    [Fact]
    public async Task DeleteMenuItem_UnknownLine_Returns404WithMessage()
    {
        var response = await Client.DeleteSnapshotAsync($"api/Order/DeleteMenuItem?id=99&guidId={_cart}");

        Assert.Equal(new HttpSnapshot(404, Text, "No menu item was found"), response);
    }
}
