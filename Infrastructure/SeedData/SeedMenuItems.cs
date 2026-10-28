using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Resturant.Domain.Entity;

namespace Resturant.Infrastructure.SeedData;

public class SeedMenuItems : IEntityTypeConfiguration<MenuItem>
{
    
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasData(
            new MenuItem() { Id = 1, Name = "Egg Roll Platter", BasePrice = 14.95m, Description = "Carrots, Tomato, Lettuce", ImageUrl = "EggRollPlatter.png"},
            new MenuItem() { Id = 2, Name = "Papaya Salad", BasePrice = 8.95m,   Description = "Green Papaya, snake beans, cherry tomato", ImageUrl = "PapayaSalad.png"},
            new MenuItem() { Id = 3, Name = "Tofu Stir Fry", BasePrice = 10.95m, Description = "Tofu, bell peppers, green onion", ImageUrl = "TofuStirFry.png"},
            new MenuItem() { Id = 4, Name = "Chopped Beef", BasePrice = 12.95m, Description = "Mushrooms, Carrots, Okra", ImageUrl =  "ChoppedBeef.png"},
            new MenuItem() { Id = 5, Name = "Veggie Platter", BasePrice = 9.95m, Description = "Edamame, Carrots, Avocado", ImageUrl = "VeggiePlatter.png"},
            new MenuItem() { Id = 6, Name = "Caesar Salad", BasePrice = 10.5m, Description = "Onion, lettuce, cheese", ImageUrl = "CaesarSalad.png" }
        );

    }
    
    
    //TODO come back to add base images in the backend instead of from the frontend in the future 
    private string CreateImageUrlPath(string menuItemName)
    {
      //  string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      //UNSURE IF THIS PATH WILL WORK IN THE BUILD 
        string baseDirectory =  Directory.GetCurrentDirectory();
        string relativePath = $"menuImages/{menuItemName}.png";
        string fullPath = Path.Combine(baseDirectory, relativePath);
        return fullPath;
    }
    
}