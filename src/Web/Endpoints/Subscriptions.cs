using ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;
using ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;
using ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;
using ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;
using ConnectFlow.Application.Subscriptions.Queries.GetSubscription;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ConnectFlow.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this).RequireAuthorization();

        group.MapGet(GetSubscription, "GetSubscription");
        group.AllowAnonymous().MapGet(GetAvailablePlans, "GetAvailablePlans");
        group.MapPost(CreateSubscription, "CreateSubscription");
        group.MapPut(UpdateSubscription, "UpdateSubscription");
        group.MapPost(CancelSubscription, "CancelSubscription");
        group.MapPost(ReactivateSubscription, "ReactivateSubscription");
        group.AllowAnonymous().MapPost(ProcessWebhook, "ProcessWebhook");
    }

    public async Task<IResult> GetSubscription(ISender sender)
    {
        var result = await sender.Send(new GetSubscriptionQuery());
        return TypedResults.Ok(result);
    }

    public async Task<IResult> GetAvailablePlans(ISender sender)
    {
        var result = await sender.Send(new GetAvailablePlansQuery());
        return TypedResults.Ok(result);
    }

    public async Task<Ok<CreateSubscriptionResult>> CreateSubscription(ISender sender, CreateSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<UpdateSubscriptionResult>> UpdateSubscription(ISender sender, UpdateSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<CancelSubscriptionResult>> CancelSubscription(ISender sender, CancelSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<ReactivateSubscriptionResult>> ReactivateSubscription(ISender sender)
    {
        var result = await sender.Send(new ReactivateSubscriptionCommand());
        return TypedResults.Ok(result);
    }

    public async Task<Ok<ProcessWebhookResult>> ProcessWebhook(HttpRequest request, ISender sender)
    {
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        var signature = request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

        var command = new ProcessWebhookCommand
        {
            Body = body,
            Signature = signature
        };

        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }
}