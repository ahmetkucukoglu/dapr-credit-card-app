namespace SharedModels.CreditCardService;

public class PayAmountRequest
{
    public decimal Amount { get; init; }
    public required string Vendor { get; init; }
    public DateTimeOffset CreatedAt { get; } = DateTime.UtcNow;
}