using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class CartContractTests : ContractTest
{
    private readonly Guid _cart = Guid.NewGuid();

    private async Task<int> FirstOrderItemIdAsync()
    {
        var items = await Client.GetSnapshotAsync($"api/Cart/GetItems?guidId={_cart}");
        return (int)items.JsonBody![0]!["id"]!;
    }

    [Fact]
    public async Task AddItem_NewCart_ReturnsBasePrice()
    {
        var response = await Client.AddToCartAsync(_cart);

        Assert.Equal(new HttpSnapshot(200, Json, """{"name":"Tofu Stir Fry","price":10.95}"""), response);
    }

    [Fact]
    public async Task AddItem_ExistingCart_ReturnsLineTotal()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.AddToCartAsync(_cart, quantity: 2, options: "Soy Sauce");

        Assert.Equal(new HttpSnapshot(200, Json, """{"name":"Tofu Stir Fry","price":22.40}"""), response);
    }

    [Fact]
    public async Task AddItem_UnknownMenuItem_Returns404WithMessage()
    {
        var response = await Client.AddToCartAsync(_cart, name: "Pizza");

        Assert.Equal(new HttpSnapshot(404, Text, "Menu item 'Pizza' was not found."), response);
    }

    [Fact]
    public async Task GetTotals_ReturnsFormattedTotals()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"api/Cart/GetTotals?guid={_cart}");

        Assert.Equal(new HttpSnapshot(200, Json, """{"subTotal":"10.95","taxAmount":"0.75","totalPrice":"11.70"}"""), response);
    }

    [Fact]
    public async Task GetTotals_UnknownCart_Returns404Problem()
    {
        var response = await Client.GetSnapshotAsync($"api/Cart/GetTotals?guid={_cart}");

        response.AssertProblem(404);
    }

    [Fact]
    public async Task GetTotals_MalformedGuid_Returns404Problem()
    {
        var response = await Client.GetSnapshotAsync("api/Cart/GetTotals?guid=not-a-guid");

        response.AssertProblem(404);
    }

    [Fact]
    public async Task CreateCart_ReturnsNewGuidAsJsonString()
    {
        var response = await Client.GetSnapshotAsync("api/Cart/CreateCart");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        Assert.True(Guid.TryParse((string)response.JsonBody!, out _));
    }

    [Theory]
    [InlineData("api/Order/GetMenuItemByGuid")]
    [InlineData("api/Order/GetMenuItemByGuidSize")]
    public async Task OldOrderGetRoutes_Return404(string route)
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"{route}?guidId={_cart}");

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Fact]
    public async Task OldShoppingCartItemsRoute_Returns404()
    {
        var response = await Client.PostSnapshotAsync("api/Order/ShoppingCartItems",
            new { guidId = _cart, name = "Tofu Stir Fry", quantity = 1, options = Array.Empty<string>() });

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Fact]
    public async Task OldDeleteMenuItemRoute_Returns404()
    {
        await Client.AddToCartAsync(_cart);
        var id = await FirstOrderItemIdAsync();

        var response = await Client.DeleteSnapshotAsync($"api/Order/DeleteMenuItem?id={id}&guidId={_cart}");

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Fact]
    public async Task GetTotalPrice_OldRoute_Returns404()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"api/Order/GetTotalPrice?guid={_cart}");

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Fact]
    public async Task GetAllTempItems_Removed_Returns404()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync("api/Order/GetAllTempItems");

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Fact]
    public async Task GetItems_ReturnsCartLinesWithOptions()
    {
        await Client.AddToCartAsync(_cart, options: "Soy Sauce");

        var response = await Client.GetSnapshotAsync($"api/Cart/GetItems?guidId={_cart}");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        var line = response.JsonBody![0]!;
        var option = line["options"]![0]!;
        Assert.Equal(
            $$"""[{"id":{{line["id"]}},"name":"Tofu Stir Fry","quantity":1,"totalItemPrice":11.20,"status":"start","options":[{"id":{{option["id"]}},"name":"Soy Sauce","price":0.25,"orderItemId":{{line["id"]}}}],"shoppingCartIdentity":"{{_cart}}"}]""",
            response.Body);
    }

    [Fact]
    public async Task GetItems_WithoutGuid_UsesDefaultAndReturnsEmptyList()
    {
        var response = await Client.GetSnapshotAsync("api/Cart/GetItems");

        Assert.Equal(new HttpSnapshot(200, Json, "[]"), response);
    }

    [Fact]
    public async Task GetItemCount_ReturnsLineCount()
    {
        await Client.AddToCartAsync(_cart);
        await Client.AddToCartAsync(_cart);

        var response = await Client.GetSnapshotAsync($"api/Cart/GetItemCount?guidId={_cart}");

        Assert.Equal(new HttpSnapshot(200, Json, "2"), response);
    }

    [Fact]
    public async Task RemoveItem_RemovesLineAndReturnsEmpty200()
    {
        await Client.AddToCartAsync(_cart);
        var id = await FirstOrderItemIdAsync();

        var response = await Client.DeleteSnapshotAsync($"api/Cart/RemoveItem?id={id}&guidId={_cart}");

        Assert.Equal(new HttpSnapshot(200, null, ""), response);
        Assert.Equal("0", (await Client.GetSnapshotAsync($"api/Cart/GetItemCount?guidId={_cart}")).Body);
    }

    [Fact]
    public async Task RemoveItem_UnknownLine_Returns404WithMessage()
    {
        var response = await Client.DeleteSnapshotAsync($"api/Cart/RemoveItem?id=99&guidId={_cart}");

        Assert.Equal(new HttpSnapshot(404, Text, "No menu item was found"), response);
    }
}
