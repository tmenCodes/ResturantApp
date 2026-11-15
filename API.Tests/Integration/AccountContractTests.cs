using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class AccountContractTests : ContractTest
{
    [Fact]
    public async Task SignOut_ReturnsMessage()
    {
        var response = await Client.PostSnapshotAsync("api/RegisterUser/SignOut", null);

        Assert.Equal(new HttpSnapshot(200, Json, """{"message":"Signed out."}"""), response);
    }

    [Fact]
    public async Task CreateUser_NormalizesAndReturnsAccount()
    {
        var response = await Client.RegisterAdaAsync();

        Assert.Equal(
            new HttpSnapshot(200, Json, """{"message":"User created successfully","username":"ada","email":"ada@example.com"}"""),
            response);
    }

    [Fact]
    public async Task CreateUser_EmailInUse_Returns400WithMessage()
    {
        await Client.RegisterAdaAsync();

        var response = await Client.RegisterAdaAsync();

        Assert.Equal(new HttpSnapshot(400, Text, "Email already in use."), response);
    }

    [Fact]
    public async Task CreateUser_MissingEmail_Returns400WithMessage()
    {
        var response = await Client.PostSnapshotAsync("api/RegisterUser/CreateUser", new { fullName = "Ada", username = "ada" });

        Assert.Equal(new HttpSnapshot(400, Text, "Email is required."), response);
    }

    [Fact]
    public async Task CreateUser_WeakPassword_Returns400WithIdentityErrors()
    {
        var response = await Client.RegisterAdaAsync(password: "short");

        Assert.Equal(400, response.Status);
        Assert.Equal(Json, response.ContentType);
        Assert.StartsWith("""[{"code":"PasswordTooShort","description":""", response.Body);
    }

    [Fact]
    public async Task Login_ReturnsTokenAndMessage()
    {
        await Client.RegisterAdaAsync();

        var response = await Client.LoginAdaAsync();

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        var token = (string)response.JsonBody!["token"]!;
        Assert.Equal($$"""{"token":"{{token}}","message":"Login successful"}""", response.Body);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401WithMessage()
    {
        await Client.RegisterAdaAsync();

        var response = await Client.LoginAdaAsync(password: "Wr0ng-password");

        Assert.Equal(new HttpSnapshot(401, Text, "Invalid email or password."), response);
    }

    [Fact]
    public async Task Login_MissingPassword_Returns400WithMessage()
    {
        var response = await Client.PostSnapshotAsync("api/RegisterUser/Login", new { email = "ada@example.com" });

        Assert.Equal(new HttpSnapshot(400, Text, "Email and password are required."), response);
    }

    [Fact]
    public async Task Login_AfterRepeatedFailures_Returns429WithMessage()
    {
        await Client.RegisterAdaAsync();
        for (var attempt = 0; attempt < 5; attempt++)
        {
            await Client.LoginAdaAsync(password: "Wr0ng-password");
        }

        var response = await Client.LoginAdaAsync();

        Assert.Equal(new HttpSnapshot(429, Text, "Account is locked. Try again later."), response);
    }

    [Fact]
    public async Task CheckDuplicateEmail_ReturnsBoolean()
    {
        Assert.Equal(new HttpSnapshot(200, Json, "false"), await Client.GetSnapshotAsync("api/RegisterUser/CheckDuplicateEmail?email=ada@example.com"));
        await Client.RegisterAdaAsync();
        Assert.Equal(new HttpSnapshot(200, Json, "true"), await Client.GetSnapshotAsync("api/RegisterUser/CheckDuplicateEmail?email=ada@example.com"));
    }

    [Fact]
    public async Task FindByUser_ReturnsBoolean()
    {
        Assert.Equal(new HttpSnapshot(200, Json, "false"), await Client.GetSnapshotAsync("api/RegisterUser/FindByUser?username=ada"));
        await Client.RegisterAdaAsync();
        Assert.Equal(new HttpSnapshot(200, Json, "true"), await Client.GetSnapshotAsync("api/RegisterUser/FindByUser?username=ada"));
    }

    [Theory]
    [InlineData("api/RegisterUser/ReturnAllUsers")]
    [InlineData("api/RegisterUser/GetTheCurrentUser")]
    [InlineData("api/RegisterUser/IsUserLoggedIn")]
    [InlineData("api/RegisterUser/user")]
    public async Task ProtectedEndpoints_WithoutToken_Return401(string url)
    {
        var response = await Client.GetSnapshotAsync(url);

        Assert.Equal(new HttpSnapshot(401, null, ""), response);
    }

    [Fact]
    public async Task ReturnAllUsers_ReturnsAccounts()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/RegisterUser/ReturnAllUsers", token);

        var id = (string)response.JsonBody![0]!["id"]!;
        Assert.Equal(
            new HttpSnapshot(200, Json, $$"""[{"id":"{{id}}","userName":"ada","email":"ada@example.com","fullName":"Ada Lovelace"}]"""),
            response);
    }

    [Fact]
    public async Task GetTheCurrentUser_ReturnsSignedInAccount()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/RegisterUser/GetTheCurrentUser", token);

        var id = (string)response.JsonBody!["id"]!;
        Assert.Equal(
            new HttpSnapshot(200, Json, $$"""{"id":"{{id}}","userName":"ada","email":"ada@example.com","fullName":"Ada Lovelace"}"""),
            response);
    }

    [Fact]
    public async Task IsUserLoggedIn_ReturnsTrue()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/RegisterUser/IsUserLoggedIn", token);

        Assert.Equal(new HttpSnapshot(200, Json, "true"), response);
    }

    [Fact]
    public async Task User_ReturnsText()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/RegisterUser/user", token);

        Assert.Equal(new HttpSnapshot(200, Text, "youur logged in bro "), response);
    }
}
