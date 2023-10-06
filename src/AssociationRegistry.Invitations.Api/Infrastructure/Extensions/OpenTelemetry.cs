using System.Reflection;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AssociationRegistry.Invitations.Api.Infrastructure.Extensions;

public static class OpenTelemetry
{
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        var collectorUrl = Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

        Action<ResourceBuilder> configureResource = r => r
            .AddService(
                serviceName,
                serviceVersion: assemblyVersion,
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant()
                        ?? "unknown",
                });

        services.ConfigureOpenTelemetryTracerProvider(
            builder =>
                builder
                    .AddSource(serviceName)
                    .ConfigureResource(configureResource).AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(
                        options =>
                        {
                            options.EnrichWithHttpRequest =
                                (activity, request) => activity.SetParentId(request.Headers["traceparent"]);
                            options.Filter = context => context.Request.Method != HttpMethods.Options;
                        })
                    .AddNpgsql()
                    .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(collectorUrl);
                        })
                    .AddSource("Wolverine"));

        services.AddLogging(
            builder =>
                builder
                    .AddOpenTelemetry(
                        options =>
                        {
                            options.IncludeScopes = true;
                            options.IncludeFormattedMessage = true;
                            options.ParseStateValues = true;

                            options.AddOtlpExporter((exporterOptions, processorOptions) =>
                            {
                                exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                                exporterOptions.Endpoint = new Uri(collectorUrl);
                            });
                        }));

        services.ConfigureOpenTelemetryMeterProvider(
            options =>
                options
                    .ConfigureResource(configureResource)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(
                        exporter =>
                        {
                            exporter.Protocol = OtlpExportProtocol.Grpc;
                            exporter.Endpoint = new Uri(collectorUrl);
                        }));

        return services;
    }
}