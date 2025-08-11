namespace ConnectFlow.Application.Common.Models;

public class EmailMessage
{
    public required string To { get; init; }
    public string[]? Cc { get; init; }
    public string[]? Bcc { get; init; }
    public required string Subject { get; init; }
    public string? Body { get; set; }
    public bool IsHtml { get; set; } = true;
    public List<EmailAttachment> Attachments { get; init; } = new();
    public string? TemplateId { get; init; }
    public Dictionary<string, object> TemplateData { get; init; } = new();
}

public class EmailAttachment
{
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = "application/octet-stream";
    public byte[] Content { get; init; } = Array.Empty<byte>();
}