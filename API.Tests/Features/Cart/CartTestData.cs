using API.Features.Cart.AddCartItem;
using Resturant.Domain.Menu;
using Resturant.Domain.Services.Ordering;
using Resturant.Infrastructure.Persistence;
using Resturant.Infrastructure.Persistence.Repositories;

namespace API.Tests.Features.Cart;

/// <summary>Shared setup for the cart handler tests: a small menu and a way to fill carts.</summary>
public static class CartTestData
{
    public const decimal TaxRate = 0.06875m;
    public static readonly DateTimeOffset Now = new(2026, 10, 28, 12, 0, 0, TimeSpan.Zero);

    public static AddCartItemHandler AddHandler(RestaurantContext context) =>
        new(new ShoppingCartRepository(context), new OrderItemFactory(new MenuRepository(context)), new EfUnitOfWork(context), new FixedTimeProvider(Now));

    public static async Task SeedMenuAsync(RestaurantContext context)
    {
        context.MenuItems.Add(new MenuItem("Tofu Stir Fry", "Tofu", 10m, "Tofu.png"));
        context.MenuItemOptions.AddRange(
            new MenuItemOption("Extra Tofu", 1m),
            new MenuItemOption("Soy Sauce", 0.5m));
        await context.SaveChangesAsync();
    }

    public static OrderItemDto Request(Guid cart, int quantity = 1, params string[] options) => new()
    {
        GuidId = cart,
        Name = "Tofu Stir Fry",
        Quantity = quantity,
        Options = options.ToList()
    };

    /// <summary>Adds a Tofu Stir Fry line to <paramref name="cart"/>.</summary>
    public static Task AddAsync(RestaurantContext context, Guid cart, int quantity = 1, params string[] options) =>
        AddHandler(context).HandleAsync(Request(cart, quantity, options));
}
