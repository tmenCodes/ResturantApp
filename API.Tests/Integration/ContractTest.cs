namespace API.Tests.Integration;

/// <summary>
/// Base for HTTP contract tests. They pin the routes, status codes, content types and bodies the Angular app
/// depends on, so they must keep passing unchanged whatever the server-side architecture is.
/// Each test gets its own API instance and database.
/// </summary>
public abstract class ContractTest : IAsyncLifetime
{
    private ApiFactory? _api;

    protected HttpClient Client { get; private set; } = null!;

    protected static readonly DateTime Now = ApiFactory.Now.UtcDateTime;

    public async Task InitializeAsync()
    {
        (_api, Client) = await ApiFactory.StartAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        if (_api is not null)
        {
            await _api.DisposeAsync();
        }
    }
}
