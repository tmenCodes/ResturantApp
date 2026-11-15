using static API.Tests.Features.UserProfile.UserProfileTestData;

namespace API.Tests.Features.UserProfile;

public class AddPaymentMethodHandlerTests
{
    [Fact]
    public async Task HandleAsync_SavesPaymentMethodForUser()
    {
        await using var context = TestDatabase.Create();

        var result = await AddHandler(context).HandleAsync("ada", Payment("Ada Lovelace"));

        var saved = Assert.Single(context.SavedPaymentMethods);
        Assert.Equal(saved.Id, result.Id);
        Assert.Equal("ada", saved.UserName);
        Assert.Equal("Visa", saved.Card.CardType);
        Assert.Equal("Ada Lovelace", result.NameonCard);
    }
}
