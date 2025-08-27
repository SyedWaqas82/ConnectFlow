namespace ConnectFlow.Domain.Events.Messaging;

public class EmailSendMessageEvent : BaseMessageEvent
{
    public required string To { get; init; }
    public string[]? Cc { get; init; }
    public string[]? Bcc { get; init; }
    public required string Subject { get; init; }
    public string? Body { get; init; }
    public bool IsHtml { get; init; } = true;
    public string? TemplateId { get; init; }
    public Dictionary<string, object> TemplateData { get; init; } = new();
    public override string MessageType => nameof(EmailSendMessageEvent);

    public EmailSendMessageEvent(int tenantId, int applicationUserId) : base(tenantId, applicationUserId)
    {
    }
}