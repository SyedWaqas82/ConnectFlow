using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;

namespace ConnectFlow.Web.Infrastructure;

public static class IEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "", ApiVersion? apiVersion = null)
    {
        Guard.Against.AnonymousMethod(handler);

        var routeBuilder = builder.MapGet(pattern, handler)
            .WithName(handler.Method.Name);

        if (apiVersion != null)
        {
            routeBuilder.MapToApiVersion(apiVersion);
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "", ApiVersion? apiVersion = null)
    {
        Guard.Against.AnonymousMethod(handler);

        var routeBuilder = builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name);

        if (apiVersion != null)
        {
            routeBuilder.MapToApiVersion(apiVersion);
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern, ApiVersion? apiVersion = null)
    {
        Guard.Against.AnonymousMethod(handler);

        var routeBuilder = builder.MapPut(pattern, handler)
            .WithName(handler.Method.Name);

        if (apiVersion != null)
        {
            routeBuilder.MapToApiVersion(apiVersion);
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern, ApiVersion? apiVersion = null)
    {
        Guard.Against.AnonymousMethod(handler);

        var routeBuilder = builder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name);

        if (apiVersion != null)
        {
            routeBuilder.MapToApiVersion(apiVersion);
        }

        return builder;
    }
}