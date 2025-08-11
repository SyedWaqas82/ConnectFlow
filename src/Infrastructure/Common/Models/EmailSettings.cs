namespace ConnectFlow.Infrastructure.Common.Models;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string FromName { get; set; } = "ConnectFlow";
    public string FromAddress { get; set; } = "no-reply@connectflow.local";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string TemplatesPath { get; set; } = "email-templates";
}
