using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class CheckoutContractTests : ContractTest
{
    private readonly Guid _cart = Guid.NewGuid();

    private string CheckoutJson(int id) =>
        $$"""{"id":{{id}},"credit":"Visa","nameonCard":"Ada","creditCardNumber":"4111","expiration":"12/30","cvv":"123","paid":true,"userProfileName":"ada","checkoutTime":"{{Now:yyyy-MM-ddTHH:mm:ss}}Z","shoppingCartIdentity":"{{_cart}}"}""";

    [Fact]
    public async Task PaymentInformation_ReturnsCheckout()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(200, Json, CheckoutJson(1)), response);
    }

    [Fact]
    public async Task PaymentInformation_CartAlreadyCheckedOut_Returns400WithMessage()
    {
        await Client.AddToCartAsync(_cart);
        await Client.CheckoutAsync(_cart.ToString());

        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(400, Text, "Another pre-existing user has that guid"), response);
    }

    [Fact]
    public async Task PaymentInformation_UnknownCart_Returns400WithMessage()
    {
        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(400, Text, "Invalid guid"), response);
    }

    [Fact]
    public async Task PaymentInformation_MalformedGuid_Returns400WithMessage()
    {
        var response = await Client.CheckoutAsync("not-a-guid");

        Assert.Equal(new HttpSnapshot(400, Text, "Invalid guid"), response);
    }

    [Fact]
    public async Task GetAllOrders_NoCheckouts_Returns400WithMessage()
    {
        var response = await Client.GetSnapshotAsync("api/Payment/GetAllOrders");

        Assert.Equal(new HttpSnapshot(400, Text, "No orders were found."), response);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsCheckouts()
    {
        await Client.AddToCartAsync(_cart);
        await Client.CheckoutAsync(_cart.ToString());

        var response = await Client.GetSnapshotAsync("api/Payment/GetAllOrders");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        var checkout = Assert.Single(response.JsonBody!.AsArray())!;
        Assert.Equal(_cart.ToString(), (string)checkout["shoppingCartIdentity"]!);
        Assert.Equal("ada", (string)checkout["userProfileName"]!);
    }

    [Fact]
    public async Task GetAllOrdersWithMenu_ReturnsPlaceholderText()
    {
        var response = await Client.GetSnapshotAsync("api/Payment/GetAllOrdersWithMenu");

        Assert.Equal(new HttpSnapshot(200, Text, "completed for now"), response);
    }
}
