namespace Resturant.Data.Entities;

public class OrderItemOption
{
    public int Id { get; set; }
    
    //name and order can be checked 
    public string Name { get; set; }
    public decimal Price { get; set; }
    //public string Type { get; set; }
     
    // Foreign key to the parent OrderItem--
        
      public OrderItem OrderItem { get; set; }  
      public int OrderItemId { get; set; }

}