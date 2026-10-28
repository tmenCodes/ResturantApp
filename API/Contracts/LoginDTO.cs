namespace API.Contracts;

/// <summary>Credentials submitted to the login endpoint.</summary>
public class LoginDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}
