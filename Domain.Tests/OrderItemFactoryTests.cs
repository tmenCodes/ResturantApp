using Domain.Tests.Fakes;
using Resturant.Domain.Menu;
using Resturant.Domain.Services.Ordering;

namespace Domain.Tests;

public class OrderItemFactoryTests
{
    private readonly FakeMenuRepository _menu = new();
    private readonly OrderItemFactory _factory;

    public OrderItemFactoryTests()
    {
        _menu.MenuItems.Add(new MenuItem { Name = "Tofu Stir Fry", Description = "Tofu", BasePrice = 10m });
        _menu.Options.Add(new MenuItemOption { Name = "Extra Tofu", Price = 1m });
        _factory = new OrderItemFactory(_menu);
    }

    [Fact]
    public async Task CreateAsync_KnownItem_BuildsPricedOrderItemAndReturnsMenuItem()
    {
        var (menuItem, orderItem) = await _factory.CreateAsync("Tofu Stir Fry", ["Extra Tofu"], 2);

        Assert.Same(_menu.MenuItems[0], menuItem);
        Assert.Equal(22m, orderItem.TotalItemPrice);
        Assert.Equal("Extra Tofu", Assert.Single(orderItem.Options).Name);
    }

    [Fact]
    public async Task CreateAsync_UnknownOption_IsIgnored()
    {
        var (_, orderItem) = await _factory.CreateAsync("Tofu Stir Fry", ["Not An Option"], 1);

        Assert.Empty(orderItem.Options);
        Assert.Equal(10m, orderItem.TotalItemPrice);
    }

    [Fact]
    public async Task CreateAsync_UnknownMenuItem_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _factory.CreateAsync("Not On The Menu", [], 1));
    }

    [Fact]
    public async Task CreateAsync_QuantityNotPositive_Throws()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _factory.CreateAsync("Tofu Stir Fry", [], 0));
    }
}
