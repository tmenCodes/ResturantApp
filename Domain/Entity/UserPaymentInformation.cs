
namespace Resturant.Domain.Entity;

public class UserPaymentInformation
{
    
    public int Id { get; init;  }
    public string? Credit { get; set;  }
    public string? NameonCard { get; set;  }
    public string? CreditCardNumber { get; set;  }
    public string? Expiration { get; set; }
    public string? CVV { get; set;  }
    public string? UserName { get; set;  }

    public UserPaymentInformation()
    {
        
    }

    public UserPaymentInformation(string credit, string nameonCard, string creditCardNumber, string expiration, string cvv, string userName)
    {
        Credit  = credit;
        NameonCard  = nameonCard;
        CreditCardNumber = creditCardNumber;
        Expiration  = expiration;
        CVV  = cvv;
        UserName   = userName;
        
    }

    
}