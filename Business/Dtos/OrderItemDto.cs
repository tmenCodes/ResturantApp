namespace Resturant.Business.Dtos;

public class OrderItemDto
{
    public Guid GuidId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public List<string> Options { get; set; } = new List<string>();
}


public class OrderItemOptionsDto
{
    public string OrderOptionsName { get; set;  }

}