namespace SharedModels;

public static class CreditCardNumberGenerator
{
    public static string Generate()
    {
        var random = new Random();
        var cardNumber = random.NextInt64(4572000000000000, 4999999999999999).ToString();

        return cardNumber;
    }
}