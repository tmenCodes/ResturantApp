using Microsoft.AspNetCore.Identity;

namespace Resturant.Infrastructure.Identity;

public class WebUser : IdentityUser
{
    public string? FullName { get; set; }
    

    
}