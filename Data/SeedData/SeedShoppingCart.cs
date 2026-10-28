using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Data.Entities;

namespace Resturant.Data.SeedData;

public class SeedShoppingCart : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasData(
            new ShoppingCart()
            {
                Id = 1,
                Identity = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                Created =  DateTime.UtcNow.ToUniversalTime(),
                TotalPrice = 0, 
                CheckoutId = null
            });
    }
}