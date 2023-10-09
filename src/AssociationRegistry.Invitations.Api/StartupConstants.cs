using System.Reflection;
using Microsoft.Net.Http.Headers;

namespace AssociationRegistry.Invitations.Api;

public static class StartupConstants
{
    public const string AllowAnyOrigin = "AllowAnyOrigin";
    public const string AllowSpecificOrigin = "AllowSpecificOrigin";

    public const string DatabaseTag = "db";

    public const string Culture = "en-GB";

    public static readonly string[] ExposedHeaders =
    {
        HeaderNames.Location, AddVersionHeaderMiddleware.HeaderName,
        AddCorrelationIdToResponseMiddleware.HeaderName,
        AddHttpSecurityHeadersMiddleware.PoweredByHeaderName,
        AddHttpSecurityHeadersMiddleware.ContentTypeOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.FrameOptionsHeaderName,
        AddHttpSecurityHeadersMiddleware.XssProtectionHeaderName,
    };

    public static readonly string[] Headers =
    {
        HeaderNames.Accept,
        HeaderNames.ContentType,
        HeaderNames.Origin,
        HeaderNames.Authorization,
        HeaderNames.IfMatch,
    };

    public static string[] HttpMethodsAsString
        => new[]
        {
            HttpMethod.Get,
            HttpMethod.Head,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Delete,
            HttpMethod.Options,
        }.Select(m => m.Method).ToArray();
}

public class AddHttpSecurityHeadersMiddleware
{
    public static string ServerHeaderName = "Server";
    public static string PoweredByHeaderName = "X-Powered-By";
    public static string ContentTypeOptionsHeaderName = "X-Content-Type-Options";
    public static string FrameOptionsHeaderName = "X-Frame-Options";
    public static string XssProtectionHeaderName = "X-XSS-Protection";

    private readonly RequestDelegate _next;
    private readonly string _serverName;
    private readonly string _poweredByName;
    private readonly FrameOptionsDirectives _frameOptionsDirectives;

    public AddHttpSecurityHeadersMiddleware(
        RequestDelegate next,
        string serverName = "Vlaamse overheid",
        string poweredByName = "Vlaamse overheid - Basisregisters Vlaanderen")
        : this(next, serverName, poweredByName, FrameOptionsDirectives.Deny)
    { }

    public AddHttpSecurityHeadersMiddleware(
        RequestDelegate next,
        string serverName = "Vlaamse overheid",
        string poweredByName = "Vlaamse overheid - Basisregisters Vlaanderen",
        FrameOptionsDirectives frameOptionsDirectives = FrameOptionsDirectives.Deny)
    {
        _next = next;
        _serverName = serverName;
        _poweredByName = poweredByName;
        _frameOptionsDirectives = frameOptionsDirectives;
    }

    public Task Invoke(HttpContext context)
    {
        context.Response.Headers.Remove(ServerHeaderName);
        context.Response.Headers.Remove(PoweredByHeaderName);

        context.Response.Headers.Add(ServerHeaderName, _serverName);
        context.Response.Headers.Add(PoweredByHeaderName, _poweredByName);
        context.Response.Headers.Add(ContentTypeOptionsHeaderName, "nosniff");

        switch (_frameOptionsDirectives)
        {
            case FrameOptionsDirectives.Deny:
                context.Response.Headers.Add(FrameOptionsHeaderName, "DENY");
                break;
            case FrameOptionsDirectives.SameOrigin:
                context.Response.Headers.Add(FrameOptionsHeaderName, "SAMEORIGIN");
                break;
            default:
                throw new ArgumentOutOfRangeException("frameOptionsDirectives", _frameOptionsDirectives, "FrameOptionsDirective can only be Deny or SameOrigin.");
        }

        context.Response.Headers.Add(XssProtectionHeaderName, "1; mode=block");

        return _next(context);
    }
}

public class AddVersionHeaderMiddleware
{
    public const string HeaderName = "x-version";

    private readonly RequestDelegate _next;
    private readonly string _headerName;

    public AddVersionHeaderMiddleware(
        RequestDelegate next,
        string headerName = HeaderName)
    {
        _next = next;
        _headerName = headerName;
    }

    public Task Invoke(HttpContext context)
    {
        var version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        context.Response.Headers.Add(_headerName, version);
        return _next(context);
    }
}

public enum FrameOptionsDirectives
{
    Deny,
    SameOrigin
}

public class AddCorrelationIdToResponseMiddleware
{
    public const string HeaderName = "x-correlation-id";

    private readonly RequestDelegate _next;
    private readonly string _headerName;

    public AddCorrelationIdToResponseMiddleware(
        RequestDelegate next,
        string headerName = HeaderName)
    {
        _next = next;
        _headerName = headerName;
    }

    public Task Invoke(HttpContext context)
    {
        context
            .Response
            .Headers
            .Add(_headerName, context.TraceIdentifier);

        return _next(context);
    }
}