namespace CreditCardService.Actors;

public class CreditCardTransactionState
{
    public decimal Amount { get; init; }
    public required string Vendor { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}