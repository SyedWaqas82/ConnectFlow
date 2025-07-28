namespace ConnectFlow.Application.Common.Interfaces;

public interface IUserService
{
    int? ApplicationUserId { get; }
    Guid? PublicUserId { get; }
    string? UserName { get; }
    IList<string> Roles { get; }

    string? GetUserName();

    bool IsInRole(string role);
}