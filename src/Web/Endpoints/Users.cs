using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Users.Commands.ConfirmEmail;
using ConnectFlow.Application.Users.Commands.CreateUser;
using ConnectFlow.Application.Users.Commands.Login;
using ConnectFlow.Application.Users.Commands.RefreshToken;
using ConnectFlow.Application.Users.Commands.ResetPassword;
using ConnectFlow.Application.Users.Commands.UpdatePassword;
using ConnectFlow.Application.Users.Queries.GetCurrentUserInformation;
using Microsoft.AspNetCore.Http.HttpResults;

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

    public async Task<Ok<Result<UserToken>>> Register(ISender sender, CreateUserCommand request)
    {
        return TypedResults.Ok(await sender.Send(request));
    }

    public async Task<Ok<Result>> ConfirmEmail(ISender sender, ConfirmEmailCommand command)
    {
        return TypedResults.Ok(await sender.Send(command));
    }

    public async Task<Ok<Result<AuthToken>>> Login(ISender sender, LoginCommand request)
    {
        return TypedResults.Ok(await sender.Send(request));
    }

    public async Task<Ok<Result<AuthToken>>> RefreshToken(ISender sender, RefreshTokenCommand command)
    {
        return TypedResults.Ok(await sender.Send(command));
    }

    public async Task<Ok<Result<UserToken>>> ResetPassword(ISender sender, ResetPasswordCommand command)
    {
        return TypedResults.Ok(await sender.Send(command));
    }

    public async Task<Ok<Result>> UpdatePassword(ISender sender, UpdatePasswordCommand command)
    {
        return TypedResults.Ok(await sender.Send(command));
    }

    public async Task<Ok<UserInformationDto>> GetCurrentUserInformation(ISender sender)
    {
        return TypedResults.Ok(await sender.Send(new GetCurrentUserInformationQuery()));
    }
}