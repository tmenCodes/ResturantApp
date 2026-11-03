using Resturant.Domain.Ordering;
using Resturant.Domain.Repositories;

namespace Domain.Tests.Fakes;

/// <summary>In-memory <see cref="IShoppingCartRepository"/> for domain service tests.</summary>
public class FakeShoppingCartRepository : IShoppingCartRepository
{
    public List<ShoppingCart> Carts { get; } = new();

    public Task<ShoppingCart?> GetCartAsync(string cartIdentity) =>
        Task.FromResult(Carts.FirstOrDefault(x => x.Identity.ToString() == cartIdentity));

    public Task<int> SaveChangesAsync() => Task.FromResult(0);

    public Task<List<OrderItem>> GetAllOrderItemsAsync() =>
        Task.FromResult(Carts.SelectMany(x => x.OrderItems).ToList());

    public Task<List<OrderItem>> GetCartOrderItemsAsync(string cartIdentity) =>
        Task.FromResult(Carts.Where(x => x.Identity.ToString() == cartIdentity).SelectMany(x => x.OrderItems).ToList());

    public Task AddCartAsync(ShoppingCart cart)
    {
        Carts.Add(cart);
        return Task.CompletedTask;
    }
}
