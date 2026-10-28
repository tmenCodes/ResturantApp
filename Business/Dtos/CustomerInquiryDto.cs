using System.ComponentModel.DataAnnotations;

namespace Resturant.Business.Dtos;

/// <summary>A contact-form inquiry, and the saved inquiry returned to the client.</summary>
public class CustomerInquiryDto
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
}
