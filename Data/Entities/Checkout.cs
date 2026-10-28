namespace Resturant.Data.Entities
{
    public class Checkout
    {
        public int Id { get; set; }
        public string? Credit { get; set; }
        public string? NameonCard { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? Expiration { get; set; }
        public string? CVV { get; set; }
        
        public bool Paid { get; set; } = false;
        
        public string?  UserProfileName { get; set; }
       // public Guid TempCartsIdentity { get; set; }
        
       public DateTime CheckoutTime { get; set; }
       
        public Guid ShoppingCartIdentity { get; set; }
        
    }
}
