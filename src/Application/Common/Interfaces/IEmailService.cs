using ConnectFlow.Application.Common.Models;

namespace ConnectFlow.Application.Common.Interfaces;

public interface IEmailService
{
    Task<Result<string>> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}