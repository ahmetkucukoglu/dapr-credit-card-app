using System.Text.Json;
using Dapr.Client;
using Dapr.Workflow;
using SharedModels;
using SharedModels.CreditCardService;

namespace StatementService.Activities.GetTransactions;

public class GetTransactionsActivity : WorkflowActivity<GetTransactionsActivityRequest, ApiResponse<List<GetTransactionsResponse>>>
{
    private readonly DaprClient _daprClient;

    public GetTransactionsActivity(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public override async Task<ApiResponse<List<GetTransactionsResponse>>> RunAsync(WorkflowActivityContext context,
        GetTransactionsActivityRequest input)
    {
        Console.WriteLine("GetTransactionsActivity: " + JsonSerializer.Serialize(input));
        
        var transactionResponse = await _daprClient.InvokeMethodAsync<ApiResponse<List<GetTransactionsResponse>>>(
            HttpMethod.Get,
            appId: Constants.CcsAppId,
            methodName: $"api/{input.CardNumber}/transactions/{input.Period}");
        
        return transactionResponse;
    }
}