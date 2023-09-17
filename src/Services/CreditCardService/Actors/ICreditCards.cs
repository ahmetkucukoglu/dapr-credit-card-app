using Dapr.Actors;

namespace CreditCardService.Actors;

public interface ICreditCards : IActor
{
    Task Register(string cardNumber);
    Task<List<string>> GetAll();
}