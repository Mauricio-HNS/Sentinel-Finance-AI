using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentinel.Application;

namespace Sentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DemoDataStore>();
        services.AddSingleton<IKnowledgeRetrievalService, FileKnowledgeRetrievalService>();
        services.AddSingleton<IEvalsTrailService, FileEvalsTrailService>();
        services.AddSingleton<IAICopilotGateway, FallbackAICopilotGateway>();
        services.AddSingleton<ISentinelReadService, DemoSentinelReadService>();
        services.AddHttpClient<IPredictionGateway, PredictionGateway>(client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalServices:PredictionBaseUrl"] ?? "http://localhost:8000");
        });
        services.AddSingleton<IExplanationGateway, FallbackExplanationGateway>();
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Sentinel.Api"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });

        return services;
    }
}
