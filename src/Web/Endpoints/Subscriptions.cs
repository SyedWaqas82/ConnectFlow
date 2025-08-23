using ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;
using ConnectFlow.Application.Subscriptions.Queries.GetSubscription;

namespace ConnectFlow.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        var secureGroup = app.MapGroup(this).RequireAuthorization();

        secureGroup.MapGet(GetSubscription, "GetSubscription");
        secureGroup.AllowAnonymous().MapGet(GetAvailablePlans, "GetAvailablePlans");
        // secureGroup.MapPost(CreateSubscription, "CreateSubscription");
        // secureGroup.MapPut(UpdateSubscription, "UpdateSubscription");
        // secureGroup.MapPost(CancelSubscription, "CancelSubscription");
        // secureGroup.MapPost(ReactivateSubscription, "ReactivateSubscription");
        // secureGroup.MapPost(ProcessWebhook, "ProcessWebhook");
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

    // public async Task<IResult> CreateSubscription(
    //     CreateSubscriptionRequest request,
    //     ISender sender)
    // {
    //     try
    //     {
    //         var command = new CreateSubscriptionCommand
    //         {
    //             PlanId = request.PlanId,
    //             SuccessUrl = request.SuccessUrl,
    //             CancelUrl = request.CancelUrl
    //         };

    //         var result = await sender.Send(command);
    //         return Results.Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.BadRequest(new { error = ex.Message });
    //     }
    // }

    // public async Task<IResult> UpdateSubscription(
    //     UpdateSubscriptionRequest request,
    //     ISender sender)
    // {
    //     try
    //     {
    //         var command = new UpdateSubscriptionCommand
    //         {
    //             NewPlanId = request.NewPlanId
    //         };

    //         var result = await sender.Send(command);
    //         return Results.Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.BadRequest(new { error = ex.Message });
    //     }
    // }

    // public async Task<IResult> CancelSubscription(
    //     CancelSubscriptionRequest request,
    //     ISender sender)
    // {
    //     try
    //     {
    //         var command = new CancelSubscriptionCommand
    //         {
    //             CancelImmediately = request.CancelImmediately
    //         };

    //         var result = await sender.Send(command);
    //         return Results.Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.BadRequest(new { error = ex.Message });
    //     }
    // }

    // public async Task<IResult> ReactivateSubscription(ISender sender)
    // {
    //     try
    //     {
    //         var command = new ReactivateSubscriptionCommand();
    //         var result = await sender.Send(command);
    //         return Results.Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.BadRequest(new { error = ex.Message });
    //     }
    // }

    // public async Task<IResult> ProcessWebhook(HttpRequest request, ISender sender)
    // {
    //     try
    //     {
    //         var body = await new StreamReader(request.Body).ReadToEndAsync();
    //         var signature = request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

    //         var command = new ProcessWebhookCommand
    //         {
    //             Body = body,
    //             Signature = signature
    //         };

    //         var result = await sender.Send(command);
    //         return Results.Ok(result);
    //     }
    //     catch (Exception ex)
    //     {
    //         return Results.BadRequest(new { error = ex.Message });
    //     }
    // }
}

public record CreateSubscriptionRequest(
    string PlanId,
    string SuccessUrl,
    string CancelUrl);

public record UpdateSubscriptionRequest(string NewPlanId);

public record CancelSubscriptionRequest(bool CancelImmediately = false);
