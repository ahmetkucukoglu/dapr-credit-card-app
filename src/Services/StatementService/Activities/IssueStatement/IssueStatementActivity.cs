using System.Text.Json;
using Dapr.Client;
using Dapr.Workflow;
using SharedModels;

namespace StatementService.Activities.IssueStatement;

public class IssueStatementActivity : WorkflowActivity<IssueStatementActivityRequest, decimal>
{
    private readonly DaprClient _daprClient;
    private const string StatementsStateStoreKey = "{0}:Statements:{1}";

    public IssueStatementActivity(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public override async Task<decimal> RunAsync(WorkflowActivityContext context,
        IssueStatementActivityRequest input)
    {
        Console.WriteLine("IssueStatementActivity: " + JsonSerializer.Serialize(input));
        
        var amount = input.Transactions.Sum(d => d.Amount);

        await SetStatementState(input.CardNumber, input.Period, amount);

        return amount;
    }

    private async Task SetStatementState(string cardNumber, string period, decimal amount)
    {
        await _daprClient.SaveStateAsync(
            storeName: Constants.StateStoreRedis,
            key: string.Format(StatementsStateStoreKey, cardNumber, period),
            value: amount);
    }
}