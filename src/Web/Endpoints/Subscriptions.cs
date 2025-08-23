using ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;
using ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;
using ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;
using ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;
using ConnectFlow.Application.Subscriptions.Queries.GetSubscription;

namespace ConnectFlow.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var group = app.MapGroup(this).RequireAuthorization();

        group.MapGet(GetSubscription, "GetSubscription");
        group.MapGet(GetAvailablePlans, "GetAvailablePlans");
        group.MapPost(CreateSubscription, "CreateSubscription");
        group.MapPut(UpdateSubscription, "UpdateSubscription");
        group.MapPost(CancelSubscription, "CancelSubscription");
        group.MapPost(ReactivateSubscription, "ReactivateSubscription");
        group.MapPost(ProcessWebhook, "ProcessWebhook");
    }

    public async Task<IResult> GetSubscription(ISender sender)
    {
        var result = await sender.Send(new GetSubscriptionQuery());
        return Results.Ok(result);
    }

    public async Task<IResult> GetAvailablePlans(ISender sender)
    {
        try
        {
            var query = new GetAvailablePlansQuery();
            var result = await sender.Send(query);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> CreateSubscription(
        CreateSubscriptionRequest request,
        ISender sender)
    {
        try
        {
            var command = new CreateSubscriptionCommand
            {
                PlanId = request.PlanId,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl
            };

            var result = await sender.Send(command);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> UpdateSubscription(
        UpdateSubscriptionRequest request,
        ISender sender)
    {
        try
        {
            var command = new UpdateSubscriptionCommand
            {
                NewPlanId = request.NewPlanId
            };

            var result = await sender.Send(command);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> CancelSubscription(
        CancelSubscriptionRequest request,
        ISender sender)
    {
        try
        {
            var command = new CancelSubscriptionCommand
            {
                CancelImmediately = request.CancelImmediately
            };

            var result = await sender.Send(command);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> ReactivateSubscription(ISender sender)
    {
        try
        {
            var command = new ReactivateSubscriptionCommand();
            var result = await sender.Send(command);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    public async Task<IResult> ProcessWebhook(HttpRequest request, ISender sender)
    {
        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            var signature = request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

            var command = new ProcessWebhookCommand
            {
                Body = body,
                Signature = signature
            };

            var result = await sender.Send(command);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }
}

public record CreateSubscriptionRequest(
    string PlanId,
    string SuccessUrl,
    string CancelUrl);

public record UpdateSubscriptionRequest(string NewPlanId);

public record CancelSubscriptionRequest(bool CancelImmediately = false);
