namespace API.Common;

/// <summary>
/// The route groups feature endpoints map onto. The prefixes are the former controller routes, so every URL the
/// frontend calls is unchanged. Each group applies the filters that reproduce <c>[ApiController]</c> behaviour.
/// </summary>
public static class ApiGroups
{
    /// <summary>Menu and cart endpoints, under <c>api/Order</c>.</summary>
    public static RouteGroupBuilder Order(IEndpointRouteBuilder app) => Create(app, "api/Order", "Order");

    /// <summary>Checkout endpoints, under <c>api/Payment</c>.</summary>
    public static RouteGroupBuilder Payment(IEndpointRouteBuilder app) => Create(app, "api/Payment", "Payment");

    /// <summary>Account endpoints, under <c>api/RegisterUser</c>.</summary>
    public static RouteGroupBuilder RegisterUser(IEndpointRouteBuilder app) => Create(app, "api/RegisterUser", "RegisterUser");

    /// <summary>Signed-in user profile endpoints, under <c>api/UserInformation</c>.</summary>
    public static RouteGroupBuilder UserInformation(IEndpointRouteBuilder app) => Create(app, "api/UserInformation", "UserInformation");

    /// <summary>Contact and utility endpoints, under <c>api/Misc</c>.</summary>
    public static RouteGroupBuilder Misc(IEndpointRouteBuilder app) => Create(app, "api/Misc", "Misc");

    private static RouteGroupBuilder Create(IEndpointRouteBuilder app, string prefix, string tag)
    {
        var group = app.MapGroup(prefix).WithTags(tag);
        group.AddEndpointFilterFactory(MvcValidationFilter.Create);
        group.AddEndpointFilter<DomainExceptionEndpointFilter>();
        return group;
    }
}
