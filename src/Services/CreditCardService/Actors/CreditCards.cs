using Dapr.Actors.Runtime;

namespace CreditCardService.Actors;

public class CreditCards : Actor, ICreditCards
{
    private const string CardNumbersStateStoreKey = "CardNumbers";

    public CreditCards(ActorHost host) : base(host)
    {
    }

    public async Task Register(string cardNumber)
    {
        var cardNumbers = await GetCardNumbers();
        cardNumbers.Add(cardNumber);

        await SetCardNumbers(cardNumbers);
    }

    public async Task<List<string>> GetAll()
    {
        var cardNumbers = await GetCardNumbers();

        return cardNumbers;
    }

    private async Task SetCardNumbers(List<string> cardNumbers)
    {
        await StateManager.SetStateAsync(CardNumbersStateStoreKey, cardNumbers);
    }

    private async Task<List<string>> GetCardNumbers()
    {
        var creditCards = new List<string>();

        var creditCardsState = await StateManager.TryGetStateAsync<List<string>>(CardNumbersStateStoreKey);

        if (creditCardsState.HasValue)
            creditCards = creditCardsState.Value;

        return creditCards;
    }
}