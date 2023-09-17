namespace SharedModels.CreditCardService;

public class CreateCreditCardRequest
{
    public required string IdentityNumber { get; init; }
    public required string FullName { get; init; }
    public decimal Limit { get; init; }
}