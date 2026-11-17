using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class CheckoutContractTests : ContractTest
{
    private readonly Guid _cart = Guid.NewGuid();

    private string CheckoutJson(int id, string userProfileName) =>
        $$"""{"id":{{id}},"credit":"Visa","nameonCard":"Ada","creditCardNumber":"4111","expiration":"12/30","cvv":"123","paid":true,"userProfileName":{{userProfileName}},"checkoutTime":"{{Now:yyyy-MM-ddTHH:mm:ss}}Z","shoppingCartIdentity":"{{_cart}}"}""";

    [Fact]
    public async Task Checkout_SignedIn_SavesOrderForTokenUser()
    {
        var token = await Client.SignInAdaAsync();
        await Client.AddToCartAsync(_cart);

        var response = await Client.CheckoutAsync(_cart.ToString(), token);

        Assert.Equal(new HttpSnapshot(200, Json, CheckoutJson(1, "\"ada\"")), response);
    }

    [Fact]
    public async Task Checkout_Guest_SavesOrderForNobody()
    {
        await Client.AddToCartAsync(_cart);

        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(200, Json, CheckoutJson(1, "null")), response);
    }

    [Fact]
    public async Task Checkout_UserNameInBody_IsIgnored()
    {
        var adaToken = await Client.SignInAdaAsync();
        await Client.AddToCartAsync(_cart);

        var response = await Client.PostSnapshotAsync("api/Cart/Checkout", new
        {
            credit = "Visa", nameonCard = "Mallory", creditCardNumber = "4111", expiration = "12/30", cvv = "123",
            userName = "ada", cartsIdentity = _cart
        });

        Assert.Null(response.JsonBody!["userProfileName"]);
        var history = await Client.GetSnapshotAsync("api/UserInformation/UserHistoryPayments", adaToken);
        Assert.Equal(new HttpSnapshot(200, Json, "[]"), history);
    }

    [Fact]
    public async Task Checkout_CartAlreadyCheckedOut_Returns400WithMessage()
    {
        await Client.AddToCartAsync(_cart);
        await Client.CheckoutAsync(_cart.ToString());

        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(400, Text, "Another pre-existing user has that guid"), response);
    }

    [Fact]
    public async Task Checkout_UnknownCart_Returns400WithMessage()
    {
        var response = await Client.CheckoutAsync(_cart.ToString());

        Assert.Equal(new HttpSnapshot(400, Text, "Invalid guid"), response);
    }

    [Fact]
    public async Task Checkout_MalformedGuid_Returns400WithMessage()
    {
        var response = await Client.CheckoutAsync("not-a-guid");

        Assert.Equal(new HttpSnapshot(400, Text, "Invalid guid"), response);
    }

    [Fact]
    public async Task GetAllOrders_WithoutToken_Returns401()
    {
        var response = await Client.GetSnapshotAsync("api/Admin/GetAllOrders");

        Assert.Equal(new HttpSnapshot(401, null, ""), response);
    }

    [Fact]
    public async Task GetAllOrders_NonAdmin_Returns403()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/Admin/GetAllOrders", token);

        Assert.Equal(new HttpSnapshot(403, null, ""), response);
    }

    [Fact]
    public async Task GetAllOrders_NoCheckouts_Returns400WithMessage()
    {
        var token = await Client.SignInSuperAdminAsync();

        var response = await Client.GetSnapshotAsync("api/Admin/GetAllOrders", token);

        Assert.Equal(new HttpSnapshot(400, Text, "No orders were found."), response);
    }

    [Fact]
    public async Task GetAllOrders_ReturnsCheckouts()
    {
        var adaToken = await Client.SignInAdaAsync();
        await Client.AddToCartAsync(_cart);
        await Client.CheckoutAsync(_cart.ToString(), adaToken);
        var token = await Client.SignInSuperAdminAsync();

        var response = await Client.GetSnapshotAsync("api/Admin/GetAllOrders", token);

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        var checkout = Assert.Single(response.JsonBody!.AsArray())!;
        Assert.Equal(_cart.ToString(), (string)checkout["shoppingCartIdentity"]!);
        Assert.Equal("ada", (string)checkout["userProfileName"]!);
    }

    [Fact]
    public async Task OldPaymentInformationRoute_Returns404()
    {
        var response = await Client.PostSnapshotAsync("api/Payment/PaymentInformation", new { cartsIdentity = _cart });

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Theory]
    [InlineData("api/Payment/GetAllOrders")]
    [InlineData("api/Payment/GetAllOrdersWithMenu")]
    public async Task RemovedPaymentRoutes_Return404(string url)
    {
        var token = await Client.SignInSuperAdminAsync();

        var response = await Client.GetSnapshotAsync(url, token);

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }
}
