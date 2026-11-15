using static API.Tests.Integration.HttpSnapshot;

namespace API.Tests.Integration;

public class ContactContractTests : ContractTest
{
    [Fact]
    public async Task BookingInformation_ReturnsSavedReservation()
    {
        var response = await Client.PostSnapshotAsync("api/Misc/BookingInformation", new
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
    public async Task CustomerInquiry_ReturnsSavedInquiry()
    {
        var response = await Client.PostSnapshotAsync("api/Misc/CustomerInquiry",
            new { id = 42, name = "Ada", email = "ada@example.com", subject = "Hours", message = "Open late?" });

        Assert.Equal(
            new HttpSnapshot(200, Json, """{"id":1,"name":"Ada","email":"ada@example.com","subject":"Hours","message":"Open late?"}"""),
            response);
    }

    [Fact]
    public async Task CustomerInquiry_MissingName_Returns400ValidationProblem()
    {
        var response = await Client.PostSnapshotAsync("api/Misc/CustomerInquiry", new { email = "ada@example.com" });

        response.AssertValidationProblem(("Name", "The Name field is required."));
    }

    [Fact]
    public async Task CreateGuideForFrontend_ReturnsNewGuidAsJsonString()
    {
        var response = await Client.GetSnapshotAsync("api/Misc/CreateGuideForFrontend");

        Assert.Equal(200, response.Status);
        Assert.Equal(Json, response.ContentType);
        Assert.True(Guid.TryParse((string)response.JsonBody!, out _));
    }

    [Fact]
    public async Task Test_ReturnsText()
    {
        var response = await Client.GetSnapshotAsync("api/Misc/Test");

        Assert.Equal(new HttpSnapshot(200, Text, "Test"), response);
    }
}
