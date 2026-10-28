namespace Resturant.Domain.Entity
{
    public class Reservation
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
}
