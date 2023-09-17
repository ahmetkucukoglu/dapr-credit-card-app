namespace SharedModels.CreditCardService;

public class GetCreditCardResponse
{
    public required string Number { get; init; }
    public required string NameOnCard { get; init; }
    public required string ExpiryDate { get; init; }
    public decimal Balance { get; init; }
    public decimal Limit { get; init; }
}