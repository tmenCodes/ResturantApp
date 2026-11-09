using API.Features.Menu.GetMenuItemOptions;
using Resturant.Infrastructure.Persistence.Repositories;

namespace API.Tests.Features.Menu;

public class GetMenuItemOptionsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsEveryOption()
    {
        await using var context = TestDatabase.Create();
        await MenuTestData.SeedAsync(context);

        var options = await new GetMenuItemOptionsHandler(new MenuRepository(context)).HandleAsync();

        var option = Assert.Single(options);
        Assert.Equal("Extra Tofu", option.Name);
        Assert.Equal(1m, option.Price);
    }
}
