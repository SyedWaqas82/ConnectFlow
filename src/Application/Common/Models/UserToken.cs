namespace ConnectFlow.Application.Common.Models;

public class UserToken
{
    public Guid ApplicationUserPublicId { get; init; }
    public string Token { get; init; } = string.Empty;
}