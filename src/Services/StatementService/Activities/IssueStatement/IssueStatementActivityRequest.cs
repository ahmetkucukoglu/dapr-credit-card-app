using SharedModels.CreditCardService;

namespace StatementService.Activities.IssueStatement;

public record IssueStatementActivityRequest(string CardNumber, string Period, List<GetTransactionsResponse> Transactions);