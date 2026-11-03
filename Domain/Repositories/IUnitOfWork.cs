namespace Resturant.Domain.Repositories;

/// <summary>Commits every change staged through the repositories in the current scope, in one transaction.</summary>
public interface IUnitOfWork
{
    /// <summary>Saves all staged additions, changes and removals, and assigns generated ids.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
