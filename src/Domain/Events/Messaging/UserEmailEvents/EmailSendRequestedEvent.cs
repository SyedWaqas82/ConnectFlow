namespace ConnectFlow.Domain.Events.UserEmailEvents;

public class EmailSendRequestedEvent : MessageBaseEvent
{
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
    //public List<EmailAttachment> Attachments { get; set; } = new();
    public string? TemplateId { get; set; }
    public Dictionary<string, object> TemplateData { get; set; } = new();
    //public EmailPriority Priority { get; set; } = EmailPriority.Normal;
    public string? Tag { get; set; }

    public override string MessageType => nameof(EmailSendRequestedEvent);
}