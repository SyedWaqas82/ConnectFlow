using ConnectFlow.Application.Common.Interfaces;
using ConnectFlow.Application.Common.Models;
using ConnectFlow.Infrastructure.Common.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ConnectFlow.Infrastructure.Services.Email;

public interface IEmailSender
{
    Task<Result<string>> SendAsync(EmailMessage message, CancellationToken ct = default);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task<Result<string>> SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        email.To.Add(MailboxAddress.Parse(message.To));
        if (message.Cc != null)
        {
            foreach (var cc in message.Cc)
            {
                if (!string.IsNullOrWhiteSpace(cc))
                    email.Cc.Add(MailboxAddress.Parse(cc));
            }
        }
        if (message.Bcc != null)
        {
            foreach (var bcc in message.Bcc)
            {
                if (!string.IsNullOrWhiteSpace(bcc))
                    email.Bcc.Add(MailboxAddress.Parse(bcc));
            }
        }
        email.Subject = message.Subject;

        var builder = new BodyBuilder();
        if (message.IsHtml)
        {
            builder.HtmlBody = message.Body;
        }
        else
        {
            builder.TextBody = message.Body;
        }

        foreach (var att in message.Attachments)
        {
            builder.Attachments.Add(att.FileName, att.Content, ContentType.Parse(att.ContentType));
        }

        email.Body = builder.ToMessageBody();

        try
        {
            using var smtp = new SmtpClient();
            var secure = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await smtp.ConnectAsync(_settings.Host, _settings.Port, secure, ct);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            }

            await smtp.SendAsync(email, ct);
            await smtp.DisconnectAsync(true, ct);

            return Result<string>.Success(email.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);

            //return the error result
            return (Result<string>)Result<string>.Failure(new[] { ex.Message });
        }
    }
}
