using Serilog.Core;
using Serilog.Events;

namespace ConnectFlow.Infrastructure.Common.Enrichers;

/// <summary>
/// Enriches Serilog events with OpenTelemetry trace and span information
/// </summary>
/// <remarks>
/// This enricher adds trace_id and span_id properties to log events, 
/// ensuring correlation between logs and traces in the observability platform.
/// </remarks>
public class TelemetryEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;

        if (activity == null)
        {
            return;
        }

        // Add trace id
        var traceId = activity.TraceId.ToString();
        if (!string.IsNullOrEmpty(traceId))
        {
            var traceIdProperty = propertyFactory.CreateProperty("trace_id", traceId);
            logEvent.AddPropertyIfAbsent(traceIdProperty);
        }

        // Add span id
        var spanId = activity.SpanId.ToString();
        if (!string.IsNullOrEmpty(spanId))
        {
            var spanIdProperty = propertyFactory.CreateProperty("span_id", spanId);
            logEvent.AddPropertyIfAbsent(spanIdProperty);
        }

        // Add the parent span id if available
        var parentSpanId = activity.ParentSpanId.ToString();
        if (!string.IsNullOrEmpty(parentSpanId))
        {
            var parentSpanIdProperty = propertyFactory.CreateProperty("parent_span_id", parentSpanId);
            logEvent.AddPropertyIfAbsent(parentSpanIdProperty);
        }
    }
}
