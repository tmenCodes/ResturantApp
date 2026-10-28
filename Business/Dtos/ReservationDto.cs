namespace Resturant.Business.Dtos;

/// <summary>A table reservation request, and the saved reservation returned to the client.</summary>
public class ReservationDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public int? People { get; set; }
    public string? Message { get; set; }
}
