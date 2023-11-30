using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text;
using AssociationRegistry.Invitations.Api.Constants;
using AssociationRegistry.Invitations.Api.Infrastructure;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Infrastructure.Filters;
using AssociationRegistry.Invitations.Api.Infrastructure.Localization;
using AssociationRegistry.Invitations.Api.Infrastructure.Swagger;
using AssociationRegistry.Invitations.Api.Uitnodigingen.StatusWijziging;
using AssociationRegistry.Invitations.Hosts.Infrastructure.Extensions;
using Be.Vlaanderen.Basisregisters.AspNetCore.Swagger;
using Destructurama;
using IdentityModel.AspNetCore.OAuth2Introspection;
using JasperFx.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NodaTime;
using Serilog;
using Serilog.Debugging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AssociationRegistry.Invitations.Api;

using Aanvragen.StatusWijziging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry = Infrastructure.Extensions.OpenTelemetry;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory(),
                WebRootPath = "wwwroot",
            });

        LoadConfiguration(builder, args);

        SelfLog.Enable(Console.WriteLine);

        ConfigureEncoding();
        ConfigureJsonSerializerSettings();
        ConfigureAppDomainExceptions();

        ConfigureKestrel(builder);
        ConfigureWebHost(builder);
        ConfigureServices(builder);
        ConfigureLogger(builder);

        var app = builder.Build();

        GlobalStringLocalizer.Instance = new GlobalStringLocalizer(app.Services);

        app
            .ConfigureDevelopmentEnvironment()
            .UseCors(StartupConstants.AllowSpecificOrigin);

        ConfigureMiddleware(app);

        ConfigureHealtChecks(app);
        ConfigureRequestLocalization(app);
        app.ConfigureAdminApiSwagger();

        // Deze volgorde is belangrijk ! DKW
        app.UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(routeBuilder => routeBuilder.MapControllers());

        ConfigureLifetimeHooks(app);

        await app.RunAsync();
    }

    private static void ConfigureRequestLocalization(WebApplication app)
    {
        var requestLocalizationOptions = app.Services
            .GetRequiredService<IOptions<RequestLocalizationOptions>>()
            .Value;

        app.UseRequestLocalization(requestLocalizationOptions);
    }

    private static void ConfigureHealtChecks(WebApplication app)
    {
        var healthCheckOptions = new HealthCheckOptions
        {
            AllowCachingResponses = false,

            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
            },

            ResponseWriter = (httpContext, healthReport) =>
            {
                httpContext.Response.ContentType = "application/json";

                var json = new JObject(
                    new JProperty(name: "status", healthReport.Status.ToString()),
                    new JProperty(name: "totalDuration", healthReport.TotalDuration.ToString()),
                    new JProperty(
                        name: "results",
                        new JObject(
                            healthReport.Entries.Select(
                                pair =>
                                    new JProperty(
                                        pair.Key,
                                        new JObject(
                                            new JProperty(name: "status", pair.Value.Status.ToString()),
                                            new JProperty(name: "duration", pair.Value.Duration),
                                            new JProperty(name: "description", pair.Value.Description),
                                            new JProperty(name: "exception", pair.Value.Exception?.Message),
                                            new JProperty(
                                                name: "data",
                                                new JObject(
                                                    pair.Value.Data.Select(
                                                        p => new JProperty(p.Key, p.Value))))))))));

                return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
            },
        };

        app.UseHealthChecks(path: "/health", healthCheckOptions);
    }

    private static void LoadConfiguration(WebApplicationBuilder builder, params string[] args)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true,
                reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .AddInMemoryCollection();
    }

    private static void ConfigureMiddleware(IApplicationBuilder app)
    {
        app
            .UseMiddleware<AddHttpSecurityHeadersMiddleware>()
            .UseMiddleware<AddVersionHeaderMiddleware>(AddVersionHeaderMiddleware.HeaderName)
            .UseResponseCompression();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        var postgreSqlOptionsSection = builder.Configuration.GetPostgreSqlOptionsSection();
        var appSettings = builder.Configuration.Get<AppSettings>();

        builder.Services
            .AddSingleton<UitnodigingsStatusHandler>()
            .AddSingleton<AanvraagStatusHandler>()
            .AddSingleton(postgreSqlOptionsSection)
            .AddSingleton(appSettings)
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddMarten(postgreSqlOptionsSection)
            .AddOpenTelemetryServices()
            .AddHttpContextAccessor()
            .AddControllers();

        builder.Services.TryAddEnumerable(ServiceDescriptor
            .Transient<IApiControllerSpecification, ApiControllerSpec>());

        builder.Services
            .AddSingleton(
                new StartupConfigureOptions
                {
                    Server =
                    {
                        BaseUrl = builder.Configuration.GetValue<string>("BaseUrl").TrimEnd(trimChar: '/'),
                    },
                });

        builder.Services
            .AddMvcCore(
                cfg =>
                {
                    cfg.RespectBrowserAcceptHeader = false;
                    cfg.ReturnHttpNotAcceptable = true;

                    cfg.Filters.Add<OperationCancelledExceptionFilter>();

                    cfg.EnableEndpointRouting = false;
                })
            .AddCors(
                cfg =>
                {
                    cfg.AddPolicy(
                        StartupConstants.AllowAnyOrigin,
                        configurePolicy: corsPolicy => corsPolicy
                            .AllowAnyOrigin()
                            .WithMethods(StartupConstants.HttpMethodsAsString)
                            .WithHeaders(StartupConstants.Headers)
                            .WithExposedHeaders(StartupConstants.ExposedHeaders)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15)));

                    cfg.AddPolicy(
                        StartupConstants.AllowSpecificOrigin,
                        configurePolicy: corsPolicy => corsPolicy
                            .WithOrigins(builder.Configuration.GetValue<string[]>("Cors") ??
                                         Array.Empty<string>())
                            .WithMethods(StartupConstants.HttpMethodsAsString)
                            .WithHeaders(StartupConstants.Headers)
                            .WithExposedHeaders(StartupConstants.ExposedHeaders)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15))
                            .AllowCredentials());
                })
            .AddControllersAsServices()
            .AddNewtonsoftJson(
                opt =>
                {
                    opt.SerializerSettings.Converters.Add(
                        new StringEnumConverter(new DefaultNamingStrategy(), allowIntegerValues: false));

                    opt.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
            .AddXmlDataContractSerializerFormatters()
            .AddFormatterMappings()
            .AddApiExplorer();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddOAuth2Introspection(
                JwtBearerDefaults.AuthenticationScheme,
                configureOptions: options =>
                {
                    var configOptions = builder.Configuration.GetSection(nameof(OAuth2IntrospectionOptions))
                        .Get<OAuth2IntrospectionOptions>()!;

                    options.ClientId = configOptions.ClientId;
                    options.ClientSecret = configOptions.ClientSecret;
                    options.Authority = configOptions.Authority;
                    options.IntrospectionEndpoint = configOptions.IntrospectionEndpoint;
                }
            );

        builder.Services
            .AddAuthorization(
                options =>
                    options.FallbackPolicy =
                        new AuthorizationPolicyBuilder()
                            .RequireClaim(Security.ClaimTypes.Scope, Security.Scopes.Uitnodigingen)
                            .Build());

        builder.Services.AddHealthChecks();


        builder.Services
            .AddLocalization(cfg => cfg.ResourcesPath = "Resources")
            .AddSingleton<IStringLocalizerFactory, SharedStringLocalizerFactory<DefaultResources>>()
            .AddSingleton<ResourceManagerStringLocalizerFactory, ResourceManagerStringLocalizerFactory>()
            .Configure<RequestLocalizationOptions>(
                opts =>
                {
                    opts.DefaultRequestCulture = new RequestCulture(new CultureInfo(StartupConstants.Culture));
                    opts.SupportedCultures = new[] { new CultureInfo(StartupConstants.Culture) };
                    opts.SupportedUICultures = new[] { new CultureInfo(StartupConstants.Culture) };

                    opts.FallBackToParentCultures = true;
                    opts.FallBackToParentUICultures = true;
                })
            .AddVersionedApiExplorer(
                cfg =>
                {
                    cfg.GroupNameFormat = "'v'VVV";
                    cfg.SubstituteApiVersionInUrl = true;
                })
            .AddApiVersioning(
                cfg =>
                {
                    cfg.ReportApiVersions = true;
                })
            .AddEndpointsApiExplorer()
            .AddResponseCompression(
                cfg =>
                {
                    cfg.EnableForHttps = true;

                    cfg.Providers.Add<BrotliCompressionProvider>();
                    cfg.Providers.Add<GzipCompressionProvider>();

                    cfg.MimeTypes = new[]
                    {
                        // General
                        "text/plain",
                        "text/csv",

                        // Static files
                        "text/css",
                        "application/javascript",

                        // MVC
                        "text/html",
                        "application/xml",
                        "text/xml",
                        "application/json",
                        "text/json",
                        "application/ld+json",
                        "application/atom+xml",

                        // Fonts
                        "application/font-woff",
                        "font/otf",
                        "application/vnd.ms-fontobject",
                    };
                })
            .Configure<GzipCompressionProviderOptions>(cfg => cfg.Level = CompressionLevel.Fastest)
            .Configure<BrotliCompressionProviderOptions>(cfg => cfg.Level = CompressionLevel.Fastest)
            .Configure<KestrelServerOptions>(serverOptions => serverOptions.AllowSynchronousIO = true);

        builder.Services.AddAdminApiSwagger(appSettings);
    }

    private static void ConfigureWebHost(WebApplicationBuilder builder)
        => builder.WebHost.CaptureStartupErrors(captureStartupErrors: true);

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        var loggerConfig =
            new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentUserName()
                .Destructure.JsonNetTypes();

        var logger = loggerConfig.CreateLogger();

        Log.Logger = logger;

        builder.Logging.AddOpenTelemetry(options =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault();
            OpenTelemetry.ConfigureResource()(resourceBuilder);
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.AddOtlpExporter((exporterOptions, _)  =>
            {
                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                exporterOptions.Endpoint = new Uri(OpenTelemetry.CollectorUrl);
            });
        });
    }

    private static void ConfigureKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(
            options =>
            {
                options.AddServerHeader = false;

                options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(value: 120);

                options.Listen(
                    new IPEndPoint(IPAddress.Any, port: 11009),
                    configure: listenOptions =>
                    {
                        listenOptions.UseConnectionLogging();
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });
            });
    }

    private static void ConfigureEncoding()
        => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    private static void ConfigureJsonSerializerSettings()
    {
        var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
        jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        JsonConvert.DefaultSettings = () => jsonSerializerSettings;
    }

    private static void ConfigureAppDomainExceptions()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
            Log.Debug(
                eventArgs.Exception,
                messageTemplate: "FirstChanceException event raised in {AppDomain}",
                AppDomain.CurrentDomain.FriendlyName);

        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            Log.Fatal(
                (Exception)eventArgs.ExceptionObject,
                messageTemplate: "Encountered a fatal exception, exiting program");
    }

    private static void ConfigureLifetimeHooks(WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started"));

        app.Lifetime.ApplicationStopping.Register(
            () =>
            {
                Log.Information("Application stopping");
                Log.CloseAndFlush();
            });

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            app.Lifetime.StopApplication();

            // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
            eventArgs.Cancel = true;
        };
    }
}

public class ApiExceptionHandler
{
}

public class JsonRequestFilter : IAsyncActionFilter
{
    private static readonly JsonLoadSettings JsonLoadSettings = new()
        { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error };

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Request.Body.Seek(offset: 0, SeekOrigin.Begin);
        var body = await context.HttpContext.Request.Body.ReadAllTextAsync();
        if (!string.IsNullOrWhiteSpace(body)) ThrowifInvalidJson(body);

        context.HttpContext.Request.Body.Seek(offset: 0, SeekOrigin.Begin);
        await next();
    }

    private static void ThrowifInvalidJson(string json)
    {
        JObject.Parse(json, JsonLoadSettings);
    }
}

public class StartupConfigureOptions
{
    public bool EnableJsonErrorActionFilter { get; set; } = false;

    public StartupConfigureOptions EnableJsonErrorActionFilterOption()
    {
        EnableJsonErrorActionFilter = true;
        return this;
    }

    public CorsOptions Cors { get; } = new();

    public class CorsOptions
    {
        public string[] Origins { get; set; } = Array.Empty<string>();
        public string[] Methods { get; set; } = Array.Empty<string>();
        public string[] Headers { get; set; } = Array.Empty<string>();
        public string[] ExposedHeaders { get; set; } = Array.Empty<string>();
    }

    public ServerOptions Server { get; } = new();

    public class ServerOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string VersionHeaderName { get; set; } = AddVersionHeaderMiddleware.HeaderName;

        public string[] MethodsToLog { get; set; } = new[]
        {
            HttpMethod.Get,
            HttpMethod.Head,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Patch,
            HttpMethod.Delete,
            HttpMethod.Options,
        }.Select(x => x.Method).ToArray();

        public string ProblemDetailsTypeNamespaceOverride { get; set; } = string.Empty;
    }

    public SwaggerOptions Swagger { get; } = new();

    public class SwaggerOptions
    {
        /// <summary>
        /// Function which returns global metadata to be included in the Swagger output.
        /// </summary>
        public Func<IApiVersionDescriptionProvider, ApiVersionDescription, OpenApiInfo> ApiInfo { get; set; } = null!;

        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files.
        /// A list of absolute paths to the files that contains XML Comments.
        /// </summary>
        public string[] XmlCommentPaths { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Easily add additional header parameters to each request.
        /// </summary>
        public IEnumerable<HeaderOperationFilter> AdditionalHeaderOperationFilters { get; set; } =
            new List<HeaderOperationFilter>();

        /// <summary>
        /// Available servers.
        /// </summary>
        public IEnumerable<Server> Servers { get; set; } = new List<Server>();


        public MiddlewareHookOptions MiddlewareHooks { get; } = new();

        public class MiddlewareHookOptions
        {
            public Action<SwaggerGenOptions>? AfterSwaggerGen { get; set; }
        }
    }

    public LocalizationOptions Localization { get; } = new();

    public class LocalizationOptions
    {
        public CultureInfo? DefaultCulture { get; set; }
        public CultureInfo[] SupportedCultures { get; set; } = Array.Empty<CultureInfo>();
    }
}
