using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Application.Tests.Http;

/// <summary>What the frontend observes from one HTTP response: status, content type and raw body.</summary>
public sealed record HttpSnapshot(int Status, string? ContentType, string Body)
{
    public const string Json = "application/json; charset=utf-8";
    public const string Text = "text/plain; charset=utf-8";
    public const string Problem = "application/problem+json; charset=utf-8";

    /// <summary>Reads the status, content type and body of a response.</summary>
    public static async Task<HttpSnapshot> OfAsync(HttpResponseMessage response) =>
        new((int)response.StatusCode, response.Content.Headers.ContentType?.ToString(), await response.Content.ReadAsStringAsync());

    /// <summary>Parses the body as JSON.</summary>
    public JsonNode? JsonBody => JsonNode.Parse(Body);

    private static readonly Dictionary<int, (string Type, string Title)> ClientErrors = new()
    {
        [400] = ("https://tools.ietf.org/html/rfc9110#section-15.5.1", "Bad Request"),
        [401] = ("https://tools.ietf.org/html/rfc9110#section-15.5.2", "Unauthorized"),
        [404] = ("https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found")
    };

    /// <summary>
    /// Asserts this is an RFC 7807 problem response with the given status and MVC's type and title for it, carrying a
    /// trace id, and returns its body.
    /// </summary>
    public JsonObject AssertProblem(int status, string? title = null)
    {
        Assert.Equal(status, Status);
        Assert.Equal(Problem, ContentType);
        var body = JsonBody!.AsObject();
        Assert.Equal(ClientErrors[status].Type, (string)body["type"]!);
        Assert.Equal(title ?? ClientErrors[status].Title, (string)body["title"]!);
        Assert.Equal(status, (int)body["status"]!);
        Assert.False(string.IsNullOrEmpty((string?)body["traceId"]));
        return body;
    }

    /// <summary>Asserts this is a 400 validation problem whose errors are exactly <paramref name="errors"/>, in order.</summary>
    public void AssertValidationProblem(params (string Key, string Message)[] errors)
    {
        var body = AssertProblem(400, "One or more validation errors occurred.");
        var actual = body["errors"]!.AsObject()
            .SelectMany(pair => pair.Value!.AsArray().Select(message => (pair.Key, (string)message!)));
        Assert.Equal(errors, actual);
    }
}

/// <summary>HTTP helpers that return <see cref="HttpSnapshot"/>s, plus the multi-step flows the contract tests share.</summary>
public static class ContractClient
{
    public const string Password = "Passw0rd!";

    public static async Task<HttpSnapshot> GetSnapshotAsync(this HttpClient client, string url, string? token = null) =>
        await SendAsync(client, new HttpRequestMessage(HttpMethod.Get, url), token);

    public static async Task<HttpSnapshot> DeleteSnapshotAsync(this HttpClient client, string url) =>
        await SendAsync(client, new HttpRequestMessage(HttpMethod.Delete, url), null);

    public static Task<HttpSnapshot> PostSnapshotAsync(this HttpClient client, string url, object? body, string? token = null) =>
        SendAsync(client, new HttpRequestMessage(HttpMethod.Post, url) { Content = JsonContent.Create(body) }, token);

    public static Task<HttpSnapshot> PutSnapshotAsync(this HttpClient client, string url, object? body, string? token = null) =>
        SendAsync(client, new HttpRequestMessage(HttpMethod.Put, url) { Content = JsonContent.Create(body) }, token);

    private static async Task<HttpSnapshot> SendAsync(HttpClient client, HttpRequestMessage request, string? token)
    {
        if (token is not null)
        {
            request.Headers.Authorization = new("Bearer", token);
        }
        using var response = await client.SendAsync(request);
        return await HttpSnapshot.OfAsync(response);
    }

    /// <summary>Adds an item to the cart and returns the response.</summary>
    public static Task<HttpSnapshot> AddToCartAsync(this HttpClient client, Guid cart, string name = "Tofu Stir Fry", int quantity = 1, params string[] options) =>
        client.PostSnapshotAsync("api/Order/ShoppingCartItems", new { guidId = cart, name, quantity, options });

    /// <summary>Checks out the cart for <paramref name="userName"/> and returns the response.</summary>
    public static Task<HttpSnapshot> CheckoutAsync(this HttpClient client, string cart, string userName = "ada") =>
        client.PostSnapshotAsync("api/Payment/PaymentInformation", new
        {
            credit = "Visa", nameonCard = "Ada", creditCardNumber = "4111", expiration = "12/30", cvv = "123",
            userName, cartsIdentity = cart
        });

    /// <summary>Registers ada@example.com and returns the response.</summary>
    public static Task<HttpSnapshot> RegisterAdaAsync(this HttpClient client, string password = Password) =>
        client.PostSnapshotAsync("api/RegisterUser/CreateUser",
            new { fullName = "Ada Lovelace", username = "Ada", email = "Ada@Example.com", password });

    /// <summary>Logs in as ada@example.com and returns the response.</summary>
    public static Task<HttpSnapshot> LoginAdaAsync(this HttpClient client, string password = Password) =>
        client.PostSnapshotAsync("api/RegisterUser/Login", new { email = "ada@example.com", password });

    /// <summary>Registers and logs in as ada@example.com and returns the bearer token.</summary>
    public static async Task<string> SignInAdaAsync(this HttpClient client)
    {
        await client.RegisterAdaAsync();
        var login = await client.LoginAdaAsync();
        return (string)login.JsonBody!["token"]!;
    }
}
