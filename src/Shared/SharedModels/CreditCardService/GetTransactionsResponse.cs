namespace SharedModels.CreditCardService;

public class GetTransactionsResponse
{
    public decimal Amount { get; init; }
    public required string Vendor { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}