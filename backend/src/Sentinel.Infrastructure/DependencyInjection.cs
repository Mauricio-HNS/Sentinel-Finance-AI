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
        var openAiOptions = new OpenAIOptions
        {
            Model = configuration["OpenAI:Model"] ?? configuration["OPENAI_MODEL"] ?? "gpt-4.1-mini",
            ApiKey = configuration["OpenAI:ApiKey"] ?? configuration["OPENAI_API_KEY"] ?? string.Empty,
            BaseUrl = configuration["OpenAI:BaseUrl"] ?? "https://api.openai.com/v1/"
        };

        services.AddSingleton<DemoDataStore>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();
        services.AddSingleton(openAiOptions);
        services.AddSingleton<FallbackAICopilotGateway>();
        services.AddSingleton<FallbackExplanationGateway>();
        services.AddSingleton<IKnowledgeRetrievalService, FileKnowledgeRetrievalService>();
        services.AddSingleton<IEvalsTrailService, FileEvalsTrailService>();
        services.AddHttpClient<OpenAiResponsesGateway>(client =>
        {
            client.BaseAddress = new Uri(openAiOptions.BaseUrl);
        });
        services.AddScoped<IAICopilotGateway>(provider => provider.GetRequiredService<OpenAiResponsesGateway>());
        services.AddScoped<ISentinelReadService, PersistentSentinelReadService>();
        services.AddHttpClient<IPredictionGateway, PredictionGateway>(client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalServices:PredictionBaseUrl"] ?? "http://localhost:8000");
        });
        services.AddScoped<IExplanationGateway>(provider => provider.GetRequiredService<OpenAiResponsesGateway>());
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
