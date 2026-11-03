namespace Resturant.Domain.Contact;

/// <summary>A customer's request to book a table.</summary>
public class Reservation
{
    private Reservation()
    {
    }

    public Reservation(string? name, string? email, string? phone, string? date, string? time, int? people, string? message)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Date = date;
        Time = time;
        People = people;
        Message = message;
    }

    public int Id { get; private set; }
    public string? Name { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Date { get; private set; }
    public string? Time { get; private set; }
    public int? People { get; private set; }
    public string? Message { get; private set; }
}
