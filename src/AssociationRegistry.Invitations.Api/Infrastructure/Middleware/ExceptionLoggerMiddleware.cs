namespace AssociationRegistry.Invitations.Api.Infrastructure.Middleware;

using JasperFx.Core;

public class ExceptionLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionLoggerMiddleware> _logger;

    public ExceptionLoggerMiddleware(RequestDelegate next, ILogger<ExceptionLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode is >= 400 and < 500 and not 404)
        {
            using var ms = new MemoryStream();
            await context.Response.Body.CopyToAsync(ms);
            var bodyContent = await ms.ReadAllTextAsync();
            _logger.LogWarning(bodyContent);
        } 
    }
}