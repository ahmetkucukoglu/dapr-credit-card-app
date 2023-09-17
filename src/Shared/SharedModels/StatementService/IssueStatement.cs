namespace SharedModels.StatementService;

public class IssueStatement
{
    public required string CardNumber { get; init; }
    public required string StatementPeriod { get; init; }
}