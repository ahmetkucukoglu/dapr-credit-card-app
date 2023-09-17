namespace CreditCardService.Actors;

public class CreditCardDetailsState
{ 
    public required string Number { get; init; }
    public required string NameOnCard { get; init; }
    public required string ExpiryDate { get; init; }
    public decimal Balance { get; set; }
    public decimal Limit { get; init; }
}