using Dapr;
using Dapr.Client;
using SharedModels;
using SharedModels.StatementService;
using StatementService.Workflows;

#pragma warning disable CS0618

namespace StatementService;

public static class WebApplicationExtensions
{
    public static void AddRoutes(this WebApplication application)
    {
        application.MapPost("issue-statements", async (DaprClient daprClient) =>
        {
            var creditCards =
                await daprClient.InvokeMethodAsync<ApiResponse<List<string>>>(
                    HttpMethod.Get,
                    appId: Constants.CcsAppId,
                    methodName: "api");

            foreach (var cardNumber in creditCards.Data)
            {
                await daprClient.PublishEventAsync(
                    pubsubName: Constants.PubSubRabbitMq,
                    topicName: Constants.IssueStatementTopic,
                    data: new IssueStatement
                    {
                        CardNumber = cardNumber,
                        StatementPeriod = DateTime.Now.ToString("MMyyyy")
                    });
            }
        });

        application.MapPost("issue-statement",
            [Topic(pubsubName: Constants.PubSubRabbitMq, name: Constants.IssueStatementTopic)]
            async (IssueStatement issueStatement, DaprClient daprClient) =>
            {
                await daprClient.StartWorkflowAsync(
                    workflowComponent: Constants.Workflow,
                    workflowName: nameof(IssueStatementWorkflow),
                    instanceId: Guid.NewGuid().ToString(),
                    input: new IssueStatementPayload(
                        issueStatement.CardNumber, issueStatement.StatementPeriod)
                );
            });
    }
}