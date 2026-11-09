using System.Text.Json;
using static API.Tests.Http.HttpSnapshot;

namespace API.Tests.Http;

public class UserProfileContractTests : ContractTest
{
    private static readonly object Card = new
    {
        cardType = "Visa", nameonCard = "Ada", creditCardNumber = "4111", expiration = "12/30", cvv = "123"
    };

    private static readonly object NewCard = new
    {
        cardType = "Amex", nameonCard = "A. Lovelace", creditCardNumber = "3782", expiration = "01/31", cvv = "9876"
    };

    private static string JsonString(string value) => JsonSerializer.Serialize(value);

    private static string PaymentMethodJson(int id) =>
        $$"""{"id":{{id}},"credit":"Visa","nameonCard":"Ada","creditCardNumber":"4111","expiration":"12/30","cvv":"123","userName":"ada"}""";

    [Theory]
    [InlineData("GET", "api/UserInformation/GetUserPaymentInformation")]
    [InlineData("GET", "api/UserInformation/UserHistoryPayments")]
    [InlineData("GET", "api/UserInformation/UserHistoryMenu")]
    [InlineData("POST", "api/UserInformation/AddUserPaymentInformation")]
    [InlineData("PUT", "api/UserInformation/UpdateUserPaymentInformation")]
    public async Task Endpoints_WithoutToken_Return401(string method, string url)
    {
        HttpSnapshot response = method switch
        {
            "POST" => await Client.PostSnapshotAsync(url, Card),
            "PUT" => await Client.PutSnapshotAsync(url, Card),
            _ => await Client.GetSnapshotAsync(url)
        };

        Assert.Equal(new HttpSnapshot(401, null, ""), response);
    }

    [Fact]
    public async Task AddUserPaymentInformation_ReturnsSavedMethod()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.PostSnapshotAsync("api/UserInformation/AddUserPaymentInformation", Card, token);

        Assert.Equal(new HttpSnapshot(200, Json, PaymentMethodJson(1)), response);
    }

    [Fact]
    public async Task GetUserPaymentInformation_None_Returns204()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/UserInformation/GetUserPaymentInformation", token);

        Assert.Equal(new HttpSnapshot(204, null, ""), response);
    }

    [Fact]
    public async Task GetUserPaymentInformation_ReturnsSavedMethod()
    {
        var token = await Client.SignInAdaAsync();
        await Client.PostSnapshotAsync("api/UserInformation/AddUserPaymentInformation", Card, token);

        var response = await Client.GetSnapshotAsync("api/UserInformation/GetUserPaymentInformation", token);

        Assert.Equal(new HttpSnapshot(200, Json, PaymentMethodJson(1)), response);
    }

    [Fact]
    public async Task UpdateUserPaymentInformation_ReturnsUpdatedMethod()
    {
        var token = await Client.SignInAdaAsync();
        await Client.PostSnapshotAsync("api/UserInformation/AddUserPaymentInformation", Card, token);

        var response = await Client.PutSnapshotAsync("api/UserInformation/UpdateUserPaymentInformation", NewCard, token);

        Assert.Equal(
            new HttpSnapshot(200, Json,
                """{"id":1,"credit":"Amex","nameonCard":"A. Lovelace","creditCardNumber":"3782","expiration":"01/31","cvv":"9876","userName":"ada"}"""),
            response);
    }

    [Fact]
    public async Task UpdateUserPaymentInformation_None_Returns400Problem()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.PutSnapshotAsync("api/UserInformation/UpdateUserPaymentInformation", NewCard, token);

        response.AssertProblem(400);
    }

    [Fact]
    public async Task UserHistoryPayments_ReturnsCheckedOutTotals()
    {
        var token = await Client.SignInAdaAsync();
        var cart = Guid.NewGuid();
        await Client.AddToCartAsync(cart);
        await Client.CheckoutAsync(cart.ToString());

        var response = await Client.GetSnapshotAsync("api/UserInformation/UserHistoryPayments", token);

        Assert.Equal(new HttpSnapshot(200, Json, $$"""[{"totalPrice":11.7028125,"date":{{JsonString(Now.ToString("d"))}}}]"""), response);
    }

    [Fact]
    public async Task UserHistoryMenu_ReturnsCheckedOutItems()
    {
        var token = await Client.SignInAdaAsync();
        var cart = Guid.NewGuid();
        await Client.AddToCartAsync(cart);
        await Client.CheckoutAsync(cart.ToString());

        var response = await Client.GetSnapshotAsync("api/UserInformation/UserHistoryMenu", token);

        Assert.Equal(
            new HttpSnapshot(200, Json, $$"""[{"date":{{JsonString(Now.ToString())}},"identity":"{{cart}}","name":"Tofu Stir Fry","price":"10.95"}]"""),
            response);
    }
}
