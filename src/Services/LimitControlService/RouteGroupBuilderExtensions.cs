using Dapr.Client;
using SharedModels;
using SharedModels.LimitControlService;

namespace LimitControlService;

public static class RouteGroupBuilderExtensions
{
    public static RouteGroupBuilder AddRoutes(this RouteGroupBuilder group)
    {
        const decimal maxLimit = 35000M;

        group.MapGet("{identityNumber}",
            async (string identityNumber, DaprClient daprClient) =>
            {
                var limitState = await daprClient.GetStateAsync<decimal?>(
                    storeName: Constants.StateStoreRedis,
                    key: identityNumber);

                if (!limitState.HasValue) return ApiResponse<decimal>.CreateSuccess(maxLimit);
                
                return ApiResponse<decimal>.CreateSuccess(limitState.Value);
            });

        group.MapPost("{identityNumber}/decrease",
            async (string identityNumber, DecreaseLimitRequest request, DaprClient daprClient) =>
            {
                var limitState = await daprClient.GetStateEntryAsync<decimal?>(
                    storeName: Constants.StateStoreRedis,
                    key: identityNumber);
                
                limitState.Value = !limitState.Value.HasValue
                    ? maxLimit - request.Limit
                    : limitState.Value - request.Limit;

                await limitState.SaveAsync();

                return ApiResponse<bool>.CreateSuccess(true);
            });

        return group;
    }
}