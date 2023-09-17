using Dapr.Workflow;
using StatementService;
using StatementService.Activities.GetTransactions;
using StatementService.Activities.IssueStatement;
using StatementService.Activities.SendEmail;
using StatementService.Workflows;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<IssueStatementWorkflow>();

    options.RegisterActivity<GetTransactionsActivity>();
    options.RegisterActivity<IssueStatementActivity>();
    options.RegisterActivity<SendEmailNotificationActivity>();
});

var app = builder.Build();

app.MapGet("/", () => "StatementService is running 🚀");

app.AddRoutes();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.Run();