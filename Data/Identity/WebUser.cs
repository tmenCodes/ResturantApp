using Microsoft.AspNetCore.Identity;

namespace Resturant.Data.Identity;

public class WebUser : IdentityUser
{
    public string? FullName { get; set; }
    

    
}