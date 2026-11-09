namespace API.Tests.Http;

/// <summary>
/// Base for HTTP contract tests. They pin the routes, status codes, content types and bodies the Angular app
/// depends on, so they must keep passing unchanged whatever the server-side architecture is.
/// Each test gets its own API instance and database.
/// </summary>
public abstract class ContractTest : IAsyncLifetime
{
    private ContractApi? _api;

    protected HttpClient Client { get; private set; } = null!;

    protected static readonly DateTime Now = ContractApi.Now.UtcDateTime;

    public async Task InitializeAsync()
    {
        (_api, Client) = await ContractApi.StartAsync();
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
