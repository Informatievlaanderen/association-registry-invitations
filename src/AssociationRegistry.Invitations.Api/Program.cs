using System.IO.Compression;
using System.Net;
using AssociationRegistry.Invitations.Api.Infrastructure.Extensions;
using AssociationRegistry.Invitations.Api.Uitnodigingen.Models;
using Destructurama;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using Serilog;
using Serilog.Debugging;

var builder = WebApplication.CreateBuilder(
    new WebApplicationOptions
    {
        Args = args,
        ContentRootPath = Directory.GetCurrentDirectory(),
        WebRootPath = "wwwroot",
    });

LoadConfiguration(builder, args);

SelfLog.Enable(Console.WriteLine);

ConfigureJsonSerializerSettings();
ConfigureAppDomainExceptions();

ConfigureKestrel(builder);
ConfigureLogger(builder);
ConfigureWebHost(builder);

ConfigureServices(builder);

builder.Services.AddAuthentication(options =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureKestrel(WebApplicationBuilder builder)
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

static void ConfigureLogger(WebApplicationBuilder builder)
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

    builder.Logging
        .AddOpenTelemetry();
}

static void LoadConfiguration(WebApplicationBuilder builder, params string[] args)
{
    builder.Configuration
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json", optional: true,
            reloadOnChange: false)
        .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables()
        .AddCommandLine(args)
        .AddInMemoryCollection();
}

static void ConfigureJsonSerializerSettings()
{
    var jsonSerializerSettings = JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureDefaultForApi();
    jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
    jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

    JsonConvert.DefaultSettings = () => jsonSerializerSettings;
}

static void ConfigureAppDomainExceptions()
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

static void ConfigureWebHost(WebApplicationBuilder builder)
    => builder.WebHost.CaptureStartupErrors(captureStartupErrors: true);

void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
    var postgreSqlOptions = webApplicationBuilder.Configuration.GetPostgreSqlOptionsSection();

    webApplicationBuilder.Services
        .AddSingleton<IClock>(SystemClock.Instance)
        .AddTransient<UitnodigingsStatusHandler>()
        .AddMarten(postgreSqlOptions)
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        
        .AddOpenTelemetryServices()
        
        .AddControllers();

    webApplicationBuilder
        .Services
        .AddMvcCore(
            cfg =>
            {
                cfg.RespectBrowserAcceptHeader = false;
                cfg.ReturnHttpNotAcceptable = true;

                cfg.EnableEndpointRouting = false;
            })
        .Services
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

}


namespace AssociationRegistry.Invitations.Api
{
    public partial class Program
    {
    }
}
