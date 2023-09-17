using System.Text.Json;
using Dapr.Client;
using Dapr.Workflow;
using SharedModels;

namespace StatementService.Activities.SendEmail;

public class SendEmailNotificationActivity : WorkflowActivity<SendEmailNotificationActivityRequest, bool>
{
    private readonly DaprClient _daprClient;

    public SendEmailNotificationActivity(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public override async Task<bool> RunAsync(WorkflowActivityContext context,
        SendEmailNotificationActivityRequest input)
    {
        Console.WriteLine("SendEmailNotificationActivity: " + JsonSerializer.Serialize(input));

        await _daprClient.InvokeBindingAsync(
            bindingName: Constants.BindingsSendGrid,
            operation: "create",
            data: @$"Your statement has been issued. <br/> <br/>
                  Card Number: **** **** **** {input.CardNumber[^4..]} <br/>
                  Period: {input.Period[..2]}/{input.Period[2..]} <br/>
                  Amount: {input.Amount:C2}",
            metadata: new Dictionary<string, string>
            {
                {"emailTo", "statement@ahmetkucukoglu.com"}, //TODO Get it from a customer service
                {"subject", $"The Your Statement for {input.Period[..2]}/{input.Period[2..]}"}
            });

        return true;
    }
}