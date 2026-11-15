using API.Features.UserProfile.GetRecentOrderTotals;
using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class GetRecentOrderTotalsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsAtMostFiveOfTheUsersOrders()
    {
        await using var context = TestDatabase.Create();
        for (var day = 1; day <= 6; day++)
        {
            await AddCheckedOutCartAsync(context, "ada", new DateTime(2026, 1, day), day);
        }
        await AddCheckedOutCartAsync(context, "grace", new DateTime(2026, 1, 1), 99m);

        var result = await new GetRecentOrderTotalsHandler(context).HandleAsync("ada");

        Assert.Equal(5, result.Count);
        Assert.DoesNotContain(result, history => history.TotalPrice == 99m);
        Assert.Equal(new DateTime(2026, 1, 1).ToString("d"), result[0].Date);
    }
}
