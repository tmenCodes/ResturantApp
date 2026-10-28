using System.ComponentModel.DataAnnotations;

namespace Resturant.Domain.Entity;



public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } 
    
    public string Description { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty;
    public decimal BasePrice { get;  set; }
    
    
}