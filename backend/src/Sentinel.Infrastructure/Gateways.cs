// ---Made By Destiny7 Softwares---
using System.Net.Http.Json;
using Sentinel.Application;

namespace Sentinel.Infrastructure;

public sealed class PredictionGateway(HttpClient httpClient) : IPredictionGateway
{
    public async Task<PredictionResponse> GetOverallRiskAsync(ScenarioSimulationRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/predict/overall-risk", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var prediction = await response.Content.ReadFromJsonAsync<PredictionResponse>(cancellationToken: cancellationToken);
        return prediction ?? new PredictionResponse("OverallRisk", 0, 0, "fallback", "Prediction service returned no content.");
    }
}

public sealed class FallbackExplanationGateway : IExplanationGateway
{
    public Task<string> GenerateAsync(string context, CancellationToken cancellationToken = default)
    {
        var summary = $"Customer shows elevated financial risk due to {context.Trim().TrimEnd('.')}.";
        return Task.FromResult(summary);
    }
}
