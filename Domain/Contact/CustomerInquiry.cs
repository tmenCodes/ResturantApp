namespace Resturant.Domain.Contact;

/// <summary>A message a customer sent through the contact form.</summary>
public class CustomerInquiry
{
    private CustomerInquiry()
    {
    }

    public CustomerInquiry(string? name, string? email, string? subject, string? message)
    {
        Name = name;
        Email = email;
        Subject = subject;
        Message = message;
    }

    public int Id { get; private set; }
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Subject { get; private set; }
    public string? Message { get; private set; }
}
