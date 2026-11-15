using System.ComponentModel.DataAnnotations;
using Resturant.Domain.ContactInfo;

namespace API.Features.Contact.CreateInquiry;

/// <summary>A contact-form inquiry, and the saved inquiry returned to the client.</summary>
public class CustomerInquiryDto
{
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }

    /// <summary>Copies a saved inquiry entity, including its generated id.</summary>
    public static CustomerInquiryDto From(CustomerInquiry entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Email = entity.Email,
        Subject = entity.Subject,
        Message = entity.Message
    };
}
