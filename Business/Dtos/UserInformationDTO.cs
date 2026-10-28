namespace Resturant.Business.Dtos
{
    public class UserInformationDTO
    {
        //  public int Id { get; set; }
        public string? Credit { get; set; }
        public string? NameonCard { get; set; }
        public string? CreditCardNumber { get; set; }
        public string? Expiration { get; set; }

        public string? CVV { get; set; }


        //one to one relationship with user dto by email....
        public string? Email { get; set; }





    }
}
