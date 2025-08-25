namespace ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;

public record ProcessWebhookCommand : IRequest<ProcessWebhookResult>
{
    public string Body { get; init; } = string.Empty;
    public string Signature { get; init; } = string.Empty;
}

public record ProcessWebhookResult
{
    public string EventId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public bool Processed { get; init; }
    public string Message { get; init; } = string.Empty;
}