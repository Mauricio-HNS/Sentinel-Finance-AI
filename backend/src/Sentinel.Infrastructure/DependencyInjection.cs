// ---Made By Destiny7 Softwares---
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentinel.Application;
using Sentinel.Infrastructure.Persistence;

namespace Sentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DemoDataStore>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();
        services.AddSingleton<IKnowledgeRetrievalService, FileKnowledgeRetrievalService>();
        services.AddSingleton<IEvalsTrailService, FileEvalsTrailService>();
        services.AddSingleton<IAICopilotGateway, FallbackAICopilotGateway>();
        services.AddScoped<ISentinelReadService, PersistentSentinelReadService>();
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
