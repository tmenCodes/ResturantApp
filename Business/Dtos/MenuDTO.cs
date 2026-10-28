namespace Resturant.Business.Dtos;

public class MenuDTO
{
    public int Id { get; set; }
    
    //we may or may not need this--but this does provided us a link between the menuItems and also the ShoppingCart!
    public string GuidId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}