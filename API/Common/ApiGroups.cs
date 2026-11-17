namespace API.Common;

/// <summary>
/// The route groups feature endpoints map onto. Most prefixes are the former controller routes; the account routes are
/// split by concern into <c>api/Registration</c>, <c>api/Login</c> and <c>api/Admin</c>. Each group applies the filters
/// that reproduce <c>[ApiController]</c> behaviour.
/// </summary>
public static class ApiGroups
{
    /// <summary>Public, read-only menu endpoints (items, add-on options, image names), under <c>api/Menu</c>.</summary>
    public static RouteGroupBuilder Menu(IEndpointRouteBuilder app) => Create(app, "api/Menu", "Menu");

    /// <summary>
    /// Cart endpoints, all keyed by the cart GUID: create a cart, add, list, count and remove items, read the totals and
    /// check out. Under <c>api/Cart</c>.
    /// </summary>
    public static RouteGroupBuilder Cart(IEndpointRouteBuilder app) => Create(app, "api/Cart", "Cart");

    /// <summary>Sign-up endpoints (create an account, check that an email or user name is free), under <c>api/Registration</c>.</summary>
    public static RouteGroupBuilder Registration(IEndpointRouteBuilder app) => Create(app, "api/Registration", "Registration");

    /// <summary>Session endpoints (log in, sign out, check the session, the signed-in account), under <c>api/Login</c>.</summary>
    public static RouteGroupBuilder Login(IEndpointRouteBuilder app) => Create(app, "api/Login", "Login");

    /// <summary>
    /// Super admin endpoints (every user, every order), under <c>api/Admin</c>. The whole group requires
    /// <see cref="AuthorizationPolicies.SuperAdmin"/>, so an endpoint mapped here can't be left public by accident.
    /// </summary>
    public static RouteGroupBuilder Admin(IEndpointRouteBuilder app) =>
        Create(app, "api/Admin", "Admin").RequireAuthorization(AuthorizationPolicies.SuperAdmin);

    /// <summary>Signed-in user profile endpoints, under <c>api/UserInformation</c>.</summary>
    public static RouteGroupBuilder UserInformation(IEndpointRouteBuilder app) => Create(app, "api/UserInformation", "UserInformation");

    /// <summary>Contact endpoints (table bookings and the contact form), under <c>api/Contact</c>.</summary>
    public static RouteGroupBuilder Contact(IEndpointRouteBuilder app) => Create(app, "api/Contact", "Contact");

    private static RouteGroupBuilder Create(IEndpointRouteBuilder app, string prefix, string tag)
    {
        var group = app.MapGroup(prefix).WithTags(tag);
        group.AddEndpointFilterFactory(MvcValidationFilter.Create);
        group.AddEndpointFilter<DomainExceptionEndpointFilter>();
        return group;
    }
}
