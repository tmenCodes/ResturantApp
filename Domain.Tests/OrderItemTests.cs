using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;

namespace Domain.Tests;

public class OrderItemTests
{
    private static readonly MenuItem TofuStirFry = new("Tofu Stir Fry", "Tofu", 10m);

    private static readonly MenuItemOption[] Options =
    [
        new("Extra Tofu", 1m),
        new("Soy Sauce", 0.5m)
    ];

    [Fact]
    public void Create_PricesBasePlusOptionsTimesQuantity()
    {
        var item = OrderItem.Create(TofuStirFry, Options, 2);

        Assert.Equal(23m, item.TotalItemPrice);
        Assert.Equal(2, item.Quantity);
        Assert.Equal("Tofu Stir Fry", item.Name);
        Assert.Equal(OrderItem.InitialStatus, item.Status);
        Assert.Equal(["Extra Tofu", "Soy Sauce"], item.Options.Select(x => x.Name));
    }

    [Fact]
    public void Create_WithoutOptions_PricesBaseTimesQuantity()
    {
        Assert.Equal(30m, OrderItem.Create(TofuStirFry, [], 3).TotalItemPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_QuantityNotPositive_Throws(int quantity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderItem.Create(TofuStirFry, Options, quantity));
    }
}
