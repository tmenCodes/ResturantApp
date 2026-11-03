using Resturant.Domain.Menu;
using Resturant.Domain.Repositories;

namespace Domain.Tests.Fakes;

/// <summary>In-memory <see cref="IMenuRepository"/> for domain service tests.</summary>
public class FakeMenuRepository : IMenuRepository
{
    public List<MenuItem> MenuItems { get; } = new();
    public List<MenuItemOption> Options { get; } = new();

    public Task<List<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(MenuItems.ToList());

    public Task<MenuItem?> GetMenuItemByNameAsync(string? name, CancellationToken cancellationToken = default) =>
        Task.FromResult(MenuItems.FirstOrDefault(x => x.Name == name));

    public Task<List<MenuItemOption>> GetOptionsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Options.ToList());

    public Task<MenuItemOption?> GetOptionByNameAsync(string? name, CancellationToken cancellationToken = default) =>
        Task.FromResult(Options.FirstOrDefault(x => x.Name == name));
}
