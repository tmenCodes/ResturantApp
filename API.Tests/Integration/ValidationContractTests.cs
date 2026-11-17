using System.Text;

namespace API.Tests.Integration;

/// <summary>Pins the <c>[ApiController]</c> validation responses: required parameters, required DTO members and empty bodies.</summary>
public class ValidationContractTests : ContractTest
{
    private async Task<HttpSnapshot> PostEmptyJsonAsync(string url)
    {
        using var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        using var response = await Client.PostAsync(url, content);
        return await HttpSnapshot.OfAsync(response);
    }

    [Theory]
    [InlineData("api/Cart/GetTotals", "guid")]
    [InlineData("api/Registration/CheckDuplicateEmail", "email")]
    [InlineData("api/Registration/FindByUser", "username")]
    public async Task MissingRequiredQueryParameter_Returns400ValidationProblem(string url, string parameter)
    {
        var response = await Client.GetSnapshotAsync(url);

        response.AssertValidationProblem((parameter, $"The {parameter} field is required."));
    }

    [Fact]
    public async Task RemoveItem_WithoutParameters_BindsDefaultsAndReturns404()
    {
        var response = await Client.DeleteSnapshotAsync("api/Cart/RemoveItem");

        Assert.Equal(new HttpSnapshot(404, HttpSnapshot.Text, "No menu item was found"), response);
    }

    [Fact]
    public async Task Checkout_WithoutCartsIdentity_Returns400ValidationProblem()
    {
        var response = await Client.PostSnapshotAsync("api/Cart/Checkout", new { credit = "Visa" });

        response.AssertValidationProblem(("CartsIdentity", "The CartsIdentity field is required."));
    }

    [Fact]
    public async Task AddItem_NullOptions_Returns400ValidationProblem()
    {
        var response = await Client.PostSnapshotAsync("api/Cart/AddItem",
            new { guidId = Guid.NewGuid(), name = "Tofu Stir Fry", quantity = 1, options = (string[]?)null });

        response.AssertValidationProblem(("Options", "The Options field is required."));
    }

    [Theory]
    [InlineData("api/Login/Login", "login")]
    [InlineData("api/Registration/CreateUser", "userDTO")]
    [InlineData("api/Cart/AddItem", "orderItemDto")]
    [InlineData("api/Cart/Checkout", "customerInformationDto")]
    [InlineData("api/Contact/MakeReservation", "bookingInformation")]
    [InlineData("api/Contact/SendInquiry", "contact")]
    public async Task EmptyBody_Returns400ValidationProblem(string url, string parameter)
    {
        var response = await PostEmptyJsonAsync(url);

        response.AssertValidationProblem(("", "A non-empty request body is required."), (parameter, $"The {parameter} field is required."));
    }

    [Theory]
    [InlineData("POST", "api/UserInformation/AddPaymentMethod")]
    [InlineData("PUT", "api/UserInformation/UpdatePaymentMethod")]
    public async Task EmptyBody_OnProfileEndpoint_Returns400ValidationProblem(string method, string url)
    {
        var token = await Client.SignInAdaAsync();
        using var request = new HttpRequestMessage(new HttpMethod(method), url)
        {
            Content = new StringContent(string.Empty, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new("Bearer", token);
        using var response = await Client.SendAsync(request);

        (await HttpSnapshot.OfAsync(response)).AssertValidationProblem(
            ("", "A non-empty request body is required."),
            ("userPaymentInformationdto", "The userPaymentInformationdto field is required."));
    }
}
