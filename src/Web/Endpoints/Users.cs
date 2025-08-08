using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Users.Commands.ConfirmEmail;
using ConnectFlow.Application.Users.Commands.CreateUser;
using ConnectFlow.Application.Users.Commands.Login;
using ConnectFlow.Application.Users.Commands.RefreshToken;
using ConnectFlow.Application.Users.Commands.ResetPassword;
using ConnectFlow.Application.Users.Commands.UpdatePassword;
using ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;

namespace ConnectFlow.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).AllowAnonymous()
            .MapPost(Register, "Register")
            .MapPost(ConfirmEmail, "ConfirmEmail")
            .MapPost(Login, "Login")
            .MapPost(RefreshToken, "Refresh")
            .MapPost(ResetPassword, "ResetPassword")
            .MapPost(UpdatePassword, "UpdatePassword");

        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCurrentUserInformation, "GetCurrentUserInformation");
    }

    public async Task<Result<UserToken>> Register(ISender sender, CreateUserCommand request)
    {
        return await sender.Send(request);
    }

    public Task<Result> ConfirmEmail(ISender sender, ConfirmEmailCommand command)
    {
        return sender.Send(command);
    }

    public async Task<Result<AuthToken>> Login(ISender sender, LoginCommand request)
    {
        return await sender.Send(request);
    }

    public Task<Result<AuthToken>> RefreshToken(ISender sender, RefreshTokenCommand command)
    {
        return sender.Send(command);
    }

    public Task<Result<UserToken>> ResetPassword(ISender sender, ResetPasswordCommand command)
    {
        return sender.Send(command);
    }

    public Task<Result> UpdatePassword(ISender sender, UpdatePasswordCommand command)
    {
        return sender.Send(command);
    }

    public Task<UserInformationDto> GetCurrentUserInformation(ISender sender)
    {
        return sender.Send(new GetCurrentUserInformationQuery());
    }
}