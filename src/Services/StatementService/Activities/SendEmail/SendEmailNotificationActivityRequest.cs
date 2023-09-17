namespace StatementService.Activities.SendEmail;

public record SendEmailNotificationActivityRequest(string CardNumber, string Period, decimal Amount);