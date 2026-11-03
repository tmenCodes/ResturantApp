using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Contact;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="CustomerInquiry"/>: every inquiry must carry a name.</summary>
public class CustomerInquiryConfiguration : IEntityTypeConfiguration<CustomerInquiry>
{
    public void Configure(EntityTypeBuilder<CustomerInquiry> builder)
    {
        builder.Property(e => e.Name).IsRequired();
    }
}
