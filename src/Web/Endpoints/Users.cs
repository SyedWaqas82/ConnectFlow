using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Users.Commands.CreateUser;
using ConnectFlow.Application.Users.Commands.Login;

namespace ConnectFlow.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).AllowAnonymous()
            .MapPost(Register, "register")
            .MapPost(Login, "login");
    }

    public async Task<Result<UserToken>> Register(ISender sender, CreateUserCommand request)
    {
        return await sender.Send(request);
    }

    public async Task<Result<AuthToken>> Login(ISender sender, LoginCommand request)
    {
        return await sender.Send(request);
    }
}
