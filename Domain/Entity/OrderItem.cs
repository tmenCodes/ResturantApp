namespace Resturant.Domain.Entity;

public class OrderItem
{
    public int Id { get; set; }
    public string Name { get; set; } 
    
    public int Quantity { get; set; }
    
    //total itemprice is calculated by calculating oreeritemoptions price and base price?
    public decimal TotalItemPrice { get;  set; }
    
    public string? Status { get;  set; } 
    
    //relationship is needed because each item has a list of options, options can only be a get requests 
    //seed, shadow property is created no, even it not on menuItemsOptions
    public List<OrderItemOption> Options { get; set; } = new(); 
    public Guid ShoppingCartIdentity { get; set; }
    public ShoppingCart? ShoppingCart { get; set; }

}