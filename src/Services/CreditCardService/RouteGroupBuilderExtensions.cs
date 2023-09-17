using CreditCardService.Actors;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using SharedModels;
using SharedModels.CreditCardService;
using SharedModels.LimitControlService;

namespace CreditCardService;

public static class RouteGroupBuilderExtensions
{
    public static RouteGroupBuilder AddRoutes(this RouteGroupBuilder group)
    {
        group.MapGet("",
            async () =>
            {
                var creditCardProxy = ActorProxy.Create<ICreditCards>(
                    actorId: new ActorId(Constants.CreditCardsActor),
                    actorType: nameof(CreditCards));

                var creditCards = await creditCardProxy.GetAll();

                return ApiResponse<List<string>>.CreateSuccess(creditCards);
            });

        group.MapPost("",
            async (
                CreateCreditCardRequest request,
                DaprClient daprClient) =>
            {
                var limitResponse = await daprClient.InvokeMethodAsync<ApiResponse<decimal>>(
                    HttpMethod.Get,
                    appId: Constants.LcsAppId,
                    methodName: $"api/{request.IdentityNumber}");

                if (!limitResponse.IsSuccess)
                    return ApiResponse.CreateFail(limitResponse.ErrorMessage ?? "Unhandled exception");
                
                if (request.Limit > limitResponse.Data)
                    return ApiResponse.CreateFail($"Limit not confirmed. Available limit is {limitResponse.Data}");

                var cardNumber = CreditCardNumberGenerator.Generate();

                var creditCardProxy = ActorProxy.Create<ICreditCard>(
                    actorId: new ActorId(cardNumber),
                    actorType: nameof(CreditCard));

                await creditCardProxy.Create(request);

                var decreaseLimitRequest = daprClient.CreateInvokeMethodRequest(
                    HttpMethod.Post,
                    appId: Constants.LcsAppId,
                    methodName: $"api/{request.IdentityNumber}/decrease",
                    data: new DecreaseLimitRequest
                    {
                        Limit = request.Limit
                    });

                await daprClient.InvokeMethodAsync<ApiResponse<bool>>(decreaseLimitRequest);

                return ApiResponse<string>.CreateSuccess(cardNumber);
            });

        group.MapPost("{cardNumber}/pay",
            async (
                string cardNumber,
                PayAmountRequest request) =>
            {
                var creditCardProxy = ActorProxy.Create<ICreditCard>(
                    actorId: new ActorId(cardNumber),
                    actorType: nameof(CreditCard));

                await creditCardProxy.PayAmount(request);

                return ApiResponse<bool>.CreateSuccess(true);
            });

        group.MapGet("{cardNumber}",
            async (
                string cardNumber) =>
            {
                var creditCardProxy = ActorProxy.Create<ICreditCard>(
                    actorId: new ActorId(cardNumber),
                    actorType: nameof(CreditCard));

                var details = await creditCardProxy.GetDetails();

                return ApiResponse<GetCreditCardResponse>.CreateSuccess(details);
            });

        group.MapGet("{cardNumber}/transactions/{period}",
            async (
                string cardNumber,
                string period) =>
            {
                var creditCardProxy = ActorProxy.Create<ICreditCard>(
                    actorId: new ActorId(cardNumber),
                    actorType: nameof(CreditCard));

                var transactions = await creditCardProxy.GetTransactions(period);

                return ApiResponse<List<GetTransactionsResponse>>.CreateSuccess(transactions);
            });

        return group;
    }
}