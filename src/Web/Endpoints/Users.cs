using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Users.Commands;

namespace ConnectFlow.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).AllowAnonymous()
            .MapPost(Register, "register")
            .MapPost(ConfirmEmail, "ConfirmEmail")
            .MapPost(Login, "login")
            .MapPost(ResetPassword, "ResetPassword")
            .MapPost(UpdatePassword, "UpdatePassword");
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

    public Task<Result<UserToken>> ResetPassword(ISender sender, ResetPasswordCommand command)
    {
        return sender.Send(command);
    }

    public Task<Result> UpdatePassword(ISender sender, UpdatePasswordCommand command)
    {
        return sender.Send(command);
    }
}
