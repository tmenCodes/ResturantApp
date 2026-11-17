using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Ordering;

namespace Resturant.Infrastructure.Persistence.Configurations;

/// <summary>Maps <see cref="Order"/>: its card is stored in the order's own columns.</summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsCardInOwnColumns(x => x.Card);
    }
}
