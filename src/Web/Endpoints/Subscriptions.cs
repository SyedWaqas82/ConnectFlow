using ConnectFlow.Application.Common.Models;
using ConnectFlow.Application.Subscriptions.Commands.CancelSubscription;
using ConnectFlow.Application.Subscriptions.Commands.CreateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.ProcessWebhook;
using ConnectFlow.Application.Subscriptions.Commands.ReactivateSubscription;
using ConnectFlow.Application.Subscriptions.Commands.UpdateSubscription;
using ConnectFlow.Application.Subscriptions.Queries.CalculateRefundCredit;
using ConnectFlow.Application.Subscriptions.Queries.GetAvailablePlans;
using ConnectFlow.Application.Subscriptions.Queries.GetCheckoutSession;
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
        group.MapGet(GetCheckoutSession, "GetCheckoutSession/{sessionId}");
        group.MapPost(CreateSubscription, "Create");
        group.MapPut(UpdateSubscription, "Update");
        group.MapPost(CancelSubscription, "Cancel");
        group.MapPost(ReactivateSubscription, "Reactivate");
        group.MapGet(CalculateRefundCredit, "CalculateRefundCredit");
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

    public async Task<Ok<CheckoutSessionStatusDto>> GetCheckoutSession(ISender sender, string sessionId)
    {
        var query = new GetCheckoutSessionQuery { SessionId = sessionId };
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<Result<CreateSubscriptionResult>>> CreateSubscription(ISender sender, CreateSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<Result<UpdateSubscriptionResult>>> UpdateSubscription(ISender sender, UpdateSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<Result<CancelSubscriptionResult>>> CancelSubscription(ISender sender, CancelSubscriptionCommand command)
    {
        var result = await sender.Send(command);
        return TypedResults.Ok(result);
    }

    public async Task<Ok<Result<ReactivateSubscriptionResult>>> ReactivateSubscription(ISender sender)
    {
        var result = await sender.Send(new ReactivateSubscriptionCommand());
        return TypedResults.Ok(result);
    }

    public async Task<Ok<decimal>> CalculateRefundCredit(ISender sender)
    {
        var result = await sender.Send(new CalculateRefundCreditQuery());
        return TypedResults.Ok(result);
    }

    public async Task<IResult> ProcessWebhook(HttpRequest request, ISender sender)
    {
        try
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            var signature = request.Headers["Stripe-Signature"].FirstOrDefault();

            // Return 400 if signature is missing (client error - don't retry)
            if (string.IsNullOrEmpty(signature))
            {
                return TypedResults.BadRequest(new { error = "Missing Stripe-Signature header" });
            }

            var command = new ProcessWebhookCommand
            {
                Body = body,
                Signature = signature
            };

            var result = await sender.Send(command);

            // Return 200 if processed successfully, regardless of whether the event was handled
            // Stripe considers any 2xx response as successful delivery
            return TypedResults.Ok(result);
        }
        catch (ArgumentException ex)
        {
            // Invalid signature or malformed webhook data (client error - don't retry)
            return TypedResults.BadRequest(new { error = "Invalid webhook signature or data", details = ex.Message });
        }
        catch (Exception ex)
        {
            // Server error - Stripe will retry
            return TypedResults.Problem(title: "Webhook processing failed", detail: ex.Message, statusCode: 500);
        }
    }
}