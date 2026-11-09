using API.Features.Account.CheckDuplicateEmail;
using API.Features.Account.FindByUser;
using API.Features.Account.GetAllUsers;
using API.Features.Account.Shared;

namespace Application.Tests.Features.Account;

public class AccountLookupHandlerTests : AccountTestHost
{
    [Fact]
    public async Task EmailExistsHandler_And_UserNameExistsHandler_ReportRegisteredAccounts()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");
        var emailExists = new EmailExistsHandler(Store);
        var userNameExists = new UserNameExistsHandler(Store);

        Assert.True(await emailExists.HandleAsync("ada@example.com"));
        Assert.False(await emailExists.HandleAsync("grace@example.com"));
        Assert.True(await userNameExists.HandleAsync("ada"));
        Assert.False(await userNameExists.HandleAsync("grace"));
        Assert.False(await userNameExists.HandleAsync(null));
    }

    [Fact]
    public async Task GetAllUsersHandler_ReturnsPublicProfiles()
    {
        await CreateAsync("Ada Lovelace", "ada", "ada@example.com");

        var user = Assert.Single(await new GetAllUsersHandler(Store).HandleAsync());

        Assert.Equal(("ada", "ada@example.com", "Ada Lovelace"), (user.UserName, user.Email, user.FullName));
    }

    [Fact]
    public async Task FindAccountHandler_ReturnsAccountOrNullWhenUnknownOrMissing()
    {
        await CreateAsync("Ada", "ada", "ada@example.com");
        var ada = Assert.Single(await new GetAllUsersHandler(Store).HandleAsync());
        var handler = new FindAccountHandler(Store);

        var found = await handler.HandleAsync(ada.Id);
        var unknown = await handler.HandleAsync(Guid.NewGuid().ToString());
        var missing = await handler.HandleAsync(null);

        Assert.Equal(ada, found);
        Assert.Null(unknown);
        Assert.Null(missing);
    }
}
