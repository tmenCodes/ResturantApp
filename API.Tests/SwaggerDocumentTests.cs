using API.Tests.Integration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace API.Tests;

public class SwaggerDocumentTests : IAsyncLifetime
{
    private ApiFactory _api = null!;
    private OpenApiDocument _document = null!;

    public async Task InitializeAsync()
    {
        (_api, _) = await ApiFactory.StartAsync();
        _document = _api.Services.GetRequiredService<ISwaggerProvider>().GetSwagger("v1");
    }

    public async Task DisposeAsync() => await _api.DisposeAsync();

    private bool RequiresBearer(string path, OperationType method) =>
        _document.Paths[path].Operations[method].Security
            .SelectMany(requirement => requirement.Keys)
            .Any(scheme => scheme.Reference.Id == JwtBearerDefaults.AuthenticationScheme);

    [Fact]
    public void Document_DefinesJwtBearerScheme()
    {
        var scheme = _document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme];

        Assert.Equal(SecuritySchemeType.Http, scheme.Type);
        Assert.Equal("bearer", scheme.Scheme);
        Assert.Equal("JWT", scheme.BearerFormat);
    }

    [Theory]
    [InlineData("/api/Admin/ReturnAllUsers")]
    [InlineData("/api/Admin/GetAllOrders")]
    [InlineData("/api/UserInformation/GetPaymentMethod")]
    public void ProtectedOperations_RequireBearer(string path)
    {
        Assert.True(RequiresBearer(path, OperationType.Get));
    }

    [Fact]
    public void Login_DoesNotRequireBearer()
    {
        Assert.False(RequiresBearer("/api/Login/Login", OperationType.Post));
    }
}
