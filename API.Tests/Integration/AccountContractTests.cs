using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class AccountContractTests : ContractTest
{
    [Fact]
    public async Task SignOut_ReturnsMessage()
    {
        var response = await Client.PostSnapshotAsync("api/Login/SignOut", null);

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
        var response = await Client.PostSnapshotAsync("api/Registration/CreateUser", new { fullName = "Ada", username = "ada" });

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
        var response = await Client.PostSnapshotAsync("api/Login/Login", new { email = "ada@example.com" });

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
        Assert.Equal(new HttpSnapshot(200, Json, "false"), await Client.GetSnapshotAsync("api/Registration/CheckDuplicateEmail?email=ada@example.com"));
        await Client.RegisterAdaAsync();
        Assert.Equal(new HttpSnapshot(200, Json, "true"), await Client.GetSnapshotAsync("api/Registration/CheckDuplicateEmail?email=ada@example.com"));
    }

    [Fact]
    public async Task FindByUser_ReturnsBoolean()
    {
        Assert.Equal(new HttpSnapshot(200, Json, "false"), await Client.GetSnapshotAsync("api/Registration/FindByUser?username=ada"));
        await Client.RegisterAdaAsync();
        Assert.Equal(new HttpSnapshot(200, Json, "true"), await Client.GetSnapshotAsync("api/Registration/FindByUser?username=ada"));
    }

    [Theory]
    [InlineData("api/Admin/ReturnAllUsers")]
    [InlineData("api/Login/IsUserLoggedIn")]
    public async Task ProtectedEndpoints_WithoutToken_Return401(string url)
    {
        var response = await Client.GetSnapshotAsync(url);

        Assert.Equal(new HttpSnapshot(401, null, ""), response);
    }

    [Fact]
    public async Task ReturnAllUsers_NonAdmin_Returns403()
    {
        await Client.PostSnapshotAsync("api/Registration/CreateUser",
            new { fullName = "Bob Builder", username = "bob", email = "bob@example.com", password = ContractClient.Password });
        var login = await Client.PostSnapshotAsync("api/Login/Login", new { email = "bob@example.com", password = ContractClient.Password });
        var token = (string)login.JsonBody!["token"]!;

        var response = await Client.GetSnapshotAsync("api/Admin/ReturnAllUsers", token);

        Assert.Equal(new HttpSnapshot(403, null, ""), response);
    }

    [Fact]
    public async Task ReturnAllUsers_Admin_ReturnsAccounts()
    {
        var token = await Client.SignInSuperAdminAsync();

        var response = await Client.GetSnapshotAsync("api/Admin/ReturnAllUsers", token);

        var id = (string)response.JsonBody![0]!["id"]!;
        Assert.Equal(
            new HttpSnapshot(200, Json, $$"""[{"id":"{{id}}","userName":"{{ApiFactory.AdminEmail}}","email":"{{ApiFactory.AdminEmail}}","fullName":"Administrator"}]"""),
            response);
    }

    [Fact]
    public async Task IsUserLoggedIn_ReturnsTrue()
    {
        var token = await Client.SignInAdaAsync();

        var response = await Client.GetSnapshotAsync("api/Login/IsUserLoggedIn", token);

        Assert.Equal(new HttpSnapshot(200, Json, "true"), response);
    }

    [Theory]
    [InlineData("api/RegisterUser/user")]
    [InlineData("api/RegisterUser/IsUserLoggedIn")]
    [InlineData("api/RegisterUser/GetTheCurrentUser")]
    [InlineData("api/RegisterUser/CheckDuplicateEmail?email=ada@example.com")]
    public async Task OldRegisterUserRoutes_Return404(string url)
    {
        var response = await Client.GetSnapshotAsync(url);

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }
}
