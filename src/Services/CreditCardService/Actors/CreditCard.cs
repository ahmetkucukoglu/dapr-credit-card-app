using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using SharedModels;
using SharedModels.CreditCardService;

namespace CreditCardService.Actors;

public class CreditCard : Actor, ICreditCard
{
    private const string DetailsStateStoreKey = "Details";
    private const string TransactionsStateStoreKey = "Transactions:{0}";

    public CreditCard(ActorHost host) : base(host)
    {
    }

    protected override Task OnActivateAsync()
    {
        Console.WriteLine($"Activating actor id: {Id}");

        return Task.CompletedTask;
    }

    protected override Task OnDeactivateAsync()
    {
        Console.WriteLine($"Deactivating actor id: {Id}");

        return Task.CompletedTask;
    }

    public async Task Create(CreateCreditCardRequest request)
    {
        await SetDetailsState(new CreditCardDetailsState
        {
            Number = Id.ToString(),
            NameOnCard = request.FullName,
            ExpiryDate = $"{DateTime.Now.Date.Month}/{(DateTime.Now.Year + 3).ToString()[2..]}",
            Balance = request.Limit,
            Limit = request.Limit
        });

        var creditCardsProxy = ActorProxy.Create<ICreditCards>(
            actorId: new ActorId(Constants.CreditCardsActor),
            actorType: nameof(CreditCards));

        await creditCardsProxy.Register(Id.ToString());
    }

    public async Task PayAmount(PayAmountRequest request)
    {
        var details = await GetDetailsState();

        if (details.Balance < request.Amount)
            throw new Exception("Balance is not available");

        details.Balance -= request.Amount;

        await SetDetailsState(details);

        var period = DateTime.Now.ToString("MMyyyy");

        var transactions = await GetTransactionsState(period);

        transactions.Add(new CreditCardTransactionState
        {
            Amount = request.Amount,
            Vendor = request.Vendor,
            CreatedAt = request.CreatedAt
        });

        await SetTransactionsState(period, transactions);
    }

    public async Task<GetCreditCardResponse> GetDetails()
    {
        var details = await GetDetailsState();

        return new GetCreditCardResponse
        {
            Number = details.Number,
            NameOnCard = details.NameOnCard,
            ExpiryDate = details.ExpiryDate,
            Balance = details.Balance,
            Limit = details.Limit
        };
    }

    public async Task<List<GetTransactionsResponse>> GetTransactions(string period)
    {
        var transactions = await GetTransactionsState(period);

        return transactions.Select(s =>
            new GetTransactionsResponse
            {
                Amount = s.Amount,
                Vendor = s.Vendor,
                CreatedAt = s.CreatedAt
            }).ToList();
    }

    private async Task SetDetailsState(CreditCardDetailsState state)
    {
        await StateManager.SetStateAsync(DetailsStateStoreKey, state);
    }

    private async Task<CreditCardDetailsState> GetDetailsState()
    {
        return await StateManager.GetStateAsync<CreditCardDetailsState>(DetailsStateStoreKey);
    }

    private async Task SetTransactionsState(string period, List<CreditCardTransactionState> state)
    {
        await StateManager.SetStateAsync(string.Format(TransactionsStateStoreKey, period), state);
    }

    private async Task<List<CreditCardTransactionState>> GetTransactionsState(string period)
    {
        var transactions = new List<CreditCardTransactionState>();

        var transactionsState =
            await StateManager.TryGetStateAsync<List<CreditCardTransactionState>>(
                string.Format(TransactionsStateStoreKey, period));

        if (transactionsState.HasValue)
            transactions = transactionsState.Value;

        return transactions;
    }
}