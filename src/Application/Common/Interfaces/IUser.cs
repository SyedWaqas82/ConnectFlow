namespace ConnectFlow.Application.Common.Interfaces;

public interface IUser
{
    int? ApplicationUserId { get; }
    Guid? PublicUserId { get; }
    string? UserName { get; }
    IList<string> Roles { get; }
}