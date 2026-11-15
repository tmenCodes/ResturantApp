using API.Features.Menu.GetImagePaths;

namespace API.Tests.Features.Menu;

public class GetImagePathsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsEveryMenuItemImage()
    {
        await using var context = TestDatabase.Create();
        await MenuTestData.SeedAsync(context);

        var urls = await new GetImagePathsHandler(context).HandleAsync();

        Assert.Equal(new[] { "PapayaSalad.png", "TofuStirFry.png" }, urls.OrderBy(x => x));
    }
}
