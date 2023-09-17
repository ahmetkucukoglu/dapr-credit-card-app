using Dapr.Actors;
using SharedModels.CreditCardService;

namespace CreditCardService.Actors;

public interface ICreditCard : IActor
{
    Task Create(CreateCreditCardRequest request);
    Task PayAmount(PayAmountRequest request);
    Task<GetCreditCardResponse> GetDetails();
    Task<List<GetTransactionsResponse>> GetTransactions(string period);
}