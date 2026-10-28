using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Entity;

namespace Resturant.Infrastructure.SeedData;

public class SeedMenuItemOptions :IEntityTypeConfiguration<MenuItemOption>

{
    public void Configure(EntityTypeBuilder<MenuItemOption> builder)
    {
        builder.HasData(
          
            new MenuItemOption() { Id = 1, Name = "Extra Chicken", Price = 1m},
            new MenuItemOption() { Id = 2, Name = "Extra Beef", Price = 1m},
            new MenuItemOption() { Id = 3, Name = "Extra Pork", Price = 1m}, 
            new MenuItemOption() { Id = 4, Name = "Extra Tofu", Price = 1m},
            new MenuItemOption() { Id = 5, Name = "Soy Sauce", Price = .25m}
        ); 
        
    }
}
