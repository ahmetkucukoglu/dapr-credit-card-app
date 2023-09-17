using System.Text.Json;
using Dapr.Workflow;
using SharedModels;
using SharedModels.CreditCardService;
using StatementService.Activities.GetTransactions;
using StatementService.Activities.IssueStatement;
using StatementService.Activities.SendEmail;

namespace StatementService.Workflows;

public class IssueStatementWorkflow : Workflow<IssueStatementPayload, bool>
{
    public override async Task<bool> RunAsync(WorkflowContext context, IssueStatementPayload input)
    {
        Console.WriteLine("Workflow Step 1: " + JsonSerializer.Serialize(input));
        context.SetCustomStatus(IssueStatementStatuses.GetTransactions);
        
        var transactionsResponse =
            await context.CallActivityAsync<ApiResponse<List<GetTransactionsResponse>>>(
                name: nameof(GetTransactionsActivity),
                input: new GetTransactionsActivityRequest(input.CardNumber, input.Period));
        
        Console.WriteLine("Workflow Step 2: " + JsonSerializer.Serialize(transactionsResponse));
        context.SetCustomStatus(IssueStatementStatuses.IssueStatement);
        
        var statementResponse =
            await context.CallActivityAsync<decimal>(
                name: nameof(IssueStatementActivity),
                input: new IssueStatementActivityRequest(
                    input.CardNumber,
                    input.Period,
                    transactionsResponse.Data));
        
        Console.WriteLine("Workflow Step 3: " + statementResponse);
        context.SetCustomStatus(IssueStatementStatuses.SendEmailNotification);

        var notificationResponse =
            await context.CallActivityAsync<bool>(
                name: nameof(SendEmailNotificationActivity),
                input: new SendEmailNotificationActivityRequest(
                    input.CardNumber,
                    input.Period,
                    statementResponse));
        
        Console.WriteLine("Workflow Step 4: " + notificationResponse);
        context.SetCustomStatus(IssueStatementStatuses.Completed);

        return true;
    }
}