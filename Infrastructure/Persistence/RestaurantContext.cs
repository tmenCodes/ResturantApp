using Microsoft.EntityFrameworkCore;

using System.Reflection.Metadata;

using Resturant.Domain.Contact;
using Resturant.Domain.Customers;
using Resturant.Domain.Menu;
using Resturant.Domain.Ordering;
using Resturant.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Resturant.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;


namespace Resturant.Infrastructure.Persistence
{
    public class RestaurantContext : IdentityDbContext<WebUser>
    {
        
        public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options)
        {
   
        }
        
        //this is needed for unit testing a parameterless constructer
        public RestaurantContext()
        {
        }

        
        public DbSet<Checkout> Checkouts { get; set; }
   
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<CustomerInquiry> CustomerInquiries { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        
        public DbSet<SavedPaymentMethod> SavedPaymentMethods { get; set; }
        
        public DbSet<OrderItem> OrderItem { get; set; }
        
        public DbSet<OrderItemOption> OrderItemOptions { get; set; }
        
        public DbSet<MenuItemOption> MenuItemOptions { get; set; }
        
        
        //change logging for entitiy framework 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
         //   optionsBuilder.EnableSensitiveDataLogging();
          //  optionsBuilder.LogTo(Console.WriteLine);    
          
          optionsBuilder.ConfigureWarnings(x => x.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShoppingCart>()
                .HasMany(e => e.OrderItems)
                .WithOne(e => e.ShoppingCart)
                .HasForeignKey(e => e.ShoppingCartIdentity)
                .HasPrincipalKey(e => e.Identity);

     
        //     modelBuilder.ApplyConfiguration(new SeedShoppingCart()); 
             modelBuilder.ApplyConfiguration(new SeedMenuItems()); 
             modelBuilder.ApplyConfiguration(new SeedMenuItemOptions());
                 
                 
        }
        
    }
}
