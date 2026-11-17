using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class ContactContractTests : ContractTest
{
    [Fact]
    public async Task MakeReservation_ReturnsSavedReservation()
    {
        var response = await Client.PostSnapshotAsync("api/Contact/MakeReservation", new
        {
            id = 42, name = "Ada", email = "ada@example.com", phone = "555", date = "2026-12-01", time = "19:00",
            people = 2, message = "Window"
        });

        Assert.Equal(
            new HttpSnapshot(200, Json,
                """{"id":1,"name":"Ada","email":"ada@example.com","phone":"555","date":"2026-12-01","time":"19:00","people":2,"message":"Window"}"""),
            response);
    }

    [Fact]
    public async Task SendInquiry_ReturnsSavedInquiry()
    {
        var response = await Client.PostSnapshotAsync("api/Contact/SendInquiry",
            new { id = 42, name = "Ada", email = "ada@example.com", subject = "Hours", message = "Open late?" });

        Assert.Equal(
            new HttpSnapshot(200, Json, """{"id":1,"name":"Ada","email":"ada@example.com","subject":"Hours","message":"Open late?"}"""),
            response);
    }

    [Fact]
    public async Task SendInquiry_MissingName_Returns400ValidationProblem()
    {
        var response = await Client.PostSnapshotAsync("api/Contact/SendInquiry", new { email = "ada@example.com" });

        response.AssertValidationProblem(("Name", "The Name field is required."));
    }

    [Theory]
    [InlineData("api/Misc/BookingInformation")]
    [InlineData("api/Misc/CustomerInquiry")]
    public async Task OldMiscPostRoutes_Return404(string url)
    {
        var response = await Client.PostSnapshotAsync(url, new { name = "Ada", email = "ada@example.com" });

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }

    [Theory]
    [InlineData("api/Misc/Test")]
    [InlineData("api/Misc/CreateGuideForFrontend")]
    public async Task OldMiscGetRoutes_Return404(string url)
    {
        var response = await Client.GetSnapshotAsync(url);

        Assert.Equal(new HttpSnapshot(404, null, ""), response);
    }
}
