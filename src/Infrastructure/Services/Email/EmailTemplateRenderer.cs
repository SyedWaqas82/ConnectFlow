using ConnectFlow.Infrastructure.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using RazorLight;

namespace ConnectFlow.Infrastructure.Services.Email;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(string templateId, object model, CancellationToken ct = default);
}

public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly ILogger<RazorEmailTemplateRenderer> _logger;
    private readonly RazorLightEngine _engine;
    private readonly string _templatesRoot;

    public RazorEmailTemplateRenderer(IOptions<EmailSettings> options, ILogger<RazorEmailTemplateRenderer> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        var settings = options.Value;

        // Determine templates root
        _templatesRoot = Path.IsPathRooted(settings.TemplatesPath)
            ? settings.TemplatesPath
            : Path.Combine(env.ContentRootPath, settings.TemplatesPath);

        if (!Directory.Exists(_templatesRoot))
        {
            Directory.CreateDirectory(_templatesRoot);
        }

        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(_templatesRoot)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderAsync(string templateId, object model, CancellationToken ct = default)
    {
        try
        {
            var templatePath = templateId.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase)
                ? templateId
                : templateId + ".cshtml";

            var key = templatePath.Replace('\\', '/');
            return await _engine.CompileRenderAsync(key, model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to render email template {TemplateId}", templateId);
            throw;
        }
    }
}
