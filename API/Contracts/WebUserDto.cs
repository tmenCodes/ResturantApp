namespace API.Contracts;

/// <summary>Registration details submitted to the create-user endpoint.</summary>
public class WebUserDto
{
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}
