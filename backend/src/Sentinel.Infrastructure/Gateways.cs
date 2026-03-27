// ---Made By Destiny7 Softwares---
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Sentinel.Application;

namespace Sentinel.Infrastructure;

public sealed class OpenAIOptions
{
    public string Model { get; init; } = "gpt-4.1-mini";
    public string ApiKey { get; init; } = string.Empty;
    public string BaseUrl { get; init; } = "https://api.openai.com/v1/";
    public string? VectorStoreId { get; init; }
    public string? RiskCopilotEvalId { get; init; }
}

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
    public string Mode => "deterministic-fallback-explainer";

    public Task<string> GenerateAsync(string context, CancellationToken cancellationToken = default)
    {
        var summary = $"Customer shows elevated financial risk due to {context.Trim().TrimEnd('.')}.";
        return Task.FromResult(summary);
    }
}

public sealed class OpenAiResponsesGateway(
    HttpClient httpClient,
    OpenAIOptions options,
    FallbackAICopilotGateway fallbackCopilotGateway,
    FallbackExplanationGateway fallbackExplanationGateway) : IAICopilotGateway, IExplanationGateway
{
    public string Mode => IsConfigured() ? "openai-responses-structured" : fallbackCopilotGateway.Mode;

    public async Task<string> GenerateAsync(string context, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured())
        {
            return await fallbackExplanationGateway.GenerateAsync(context, cancellationToken);
        }

        var payload = new
        {
            model = options.Model,
            input = new object[]
            {
                new
                {
                    role = "system",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text = "You are Sentinel Finance AI, a financial risk intelligence copilot. Produce concise, professional executive explanations."
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text = $"Generate a short executive explanation for this risk context: {context}"
                        }
                    }
                }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "risk_explanation",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new
                        {
                            explanation = new { type = "string" },
                            tone = new { type = "string" }
                        },
                        required = new[] { "explanation", "tone" }
                    }
                }
            }
        };

        try
        {
            var document = await SendResponsesRequestAsync(payload, cancellationToken);
            if (document is null)
            {
                return await fallbackExplanationGateway.GenerateAsync(context, cancellationToken);
            }

            var raw = ExtractResponseText(document.RootElement);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return await fallbackExplanationGateway.GenerateAsync(context, cancellationToken);
            }

            using var parsed = JsonDocument.Parse(raw);
            return parsed.RootElement.TryGetProperty("explanation", out var explanationElement)
                ? explanationElement.GetString() ?? await fallbackExplanationGateway.GenerateAsync(context, cancellationToken)
                : await fallbackExplanationGateway.GenerateAsync(context, cancellationToken);
        }
        catch
        {
            return await fallbackExplanationGateway.GenerateAsync(context, cancellationToken);
        }
    }

    public async Task<CopilotResponseDto> GenerateAsync(
        CustomerDetailDto detail,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge,
        CancellationToken cancellationToken = default)
    {
        if (!IsConfigured())
        {
            return await fallbackCopilotGateway.GenerateAsync(detail, question, knowledge, cancellationToken);
        }

        var payload = new
        {
            model = options.Model,
            input = new object[]
            {
                new
                {
                    role = "system",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text = "You are Sentinel Finance AI, an enterprise financial risk copilot. Return strictly valid JSON. Focus on executive clarity, auditability, and actionability."
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "input_text",
                            text = BuildCopilotPrompt(detail, question, knowledge)
                        }
                    }
                }
            },
            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "risk_copilot_response",
                    strict = true,
                    schema = new
                    {
                        type = "object",
                        additionalProperties = false,
                        properties = new
                        {
                            answer = new { type = "string" },
                            executiveSummary = new { type = "string" },
                            riskLevel = new { type = "string" },
                            confidence = new { type = "number" },
                            topSignals = new
                            {
                                type = "array",
                                items = new
                                {
                                    type = "object",
                                    additionalProperties = false,
                                    properties = new
                                    {
                                        label = new { type = "string" },
                                        value = new { type = "string" },
                                        impact = new { type = "string" }
                                    },
                                    required = new[] { "label", "value", "impact" }
                                }
                            },
                            recommendedActions = new
                            {
                                type = "array",
                                items = new { type = "string" }
                            },
                            followUpQuestions = new
                            {
                                type = "array",
                                items = new { type = "string" }
                            }
                        },
                        required = new[]
                        {
                            "answer", "executiveSummary", "riskLevel", "confidence", "topSignals", "recommendedActions", "followUpQuestions"
                        }
                    }
                }
            }
        };

        try
        {
            var document = await SendResponsesRequestAsync(payload, cancellationToken);
            if (document is null)
            {
                return await fallbackCopilotGateway.GenerateAsync(detail, question, knowledge, cancellationToken);
            }

            var raw = ExtractResponseText(document.RootElement);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return await fallbackCopilotGateway.GenerateAsync(detail, question, knowledge, cancellationToken);
            }

            using var parsed = JsonDocument.Parse(raw);
            return MapCopilotResponse(detail.Customer.Id, question, knowledge, parsed.RootElement);
        }
        catch
        {
            return await fallbackCopilotGateway.GenerateAsync(detail, question, knowledge, cancellationToken);
        }
    }

    private bool IsConfigured() => !string.IsNullOrWhiteSpace(options.ApiKey);

    private async Task<JsonDocument?> SendResponsesRequestAsync(object payload, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "responses");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(content) ? null : JsonDocument.Parse(content);
    }

    private static string BuildCopilotPrompt(
        CustomerDetailDto detail,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge)
    {
        var latestPayment = detail.Payments.OrderByDescending(x => x.DueDate).FirstOrDefault();
        var latestUsage = detail.UsageMetrics.OrderByDescending(x => x.ReferenceMonth).FirstOrDefault();
        var criticalTickets = detail.Tickets.Count(x => x.Severity == "Critical");

        var context = new
        {
            customer = new
            {
                detail.Customer.Name,
                detail.Customer.Segment,
                detail.Customer.Industry,
                detail.Customer.Country,
                detail.Customer.CurrentPlan,
                detail.Customer.MonthlyRevenue,
                detail.Customer.ContractEndDate
            },
            latestRiskAssessment = new
            {
                detail.LatestRiskAssessment.OverallRiskScore,
                detail.LatestRiskAssessment.ChurnRiskScore,
                detail.LatestRiskAssessment.LatePaymentRiskScore,
                detail.LatestRiskAssessment.RevenueRiskScore,
                RiskLevel = detail.LatestRiskAssessment.RiskLevel.ToString(),
                detail.LatestRiskAssessment.Summary
            },
            latestSignals = new
            {
                LatestDaysLate = latestPayment?.DaysLate ?? 0,
                LatestPaymentStatus = latestPayment?.Status ?? "Unknown",
                UsageVariationPercent = latestUsage?.UsageVariationPercent ?? 0,
                FeatureUsageScore = latestUsage?.FeatureUsageScore ?? 0,
                CriticalTickets = criticalTickets,
                OpenAlerts = detail.Alerts.Count(x => !x.IsResolved)
            },
            retrievedKnowledge = knowledge.Select(chunk => new
            {
                chunk.Title,
                chunk.SourceType,
                chunk.Snippet,
                chunk.Score
            })
        };

        return $$"""
        Answer the following question about the customer portfolio risk:
        {{question}}

        Use the structured customer context below. Keep the answer executive, specific, and operationally useful.
        Recommend actions that a revenue, collections, or customer success leader could execute this week.

        {{JsonSerializer.Serialize(context)}}
        """;
    }

    private static string? ExtractResponseText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputText) && outputText.ValueKind == JsonValueKind.String)
        {
            return outputText.GetString();
        }

        if (!root.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var textElement) && textElement.ValueKind == JsonValueKind.String)
                {
                    return textElement.GetString();
                }
            }
        }

        return null;
    }

    private static CopilotResponseDto MapCopilotResponse(
        Guid customerId,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge,
        JsonElement root)
    {
        var answer = root.TryGetProperty("answer", out var answerElement)
            ? answerElement.GetString() ?? "No answer returned."
            : "No answer returned.";
        var executiveSummary = root.TryGetProperty("executiveSummary", out var summaryElement)
            ? summaryElement.GetString() ?? "No summary returned."
            : "No summary returned.";
        var riskLevel = root.TryGetProperty("riskLevel", out var riskLevelElement)
            ? riskLevelElement.GetString() ?? "Unknown"
            : "Unknown";
        var confidence = root.TryGetProperty("confidence", out var confidenceElement) && confidenceElement.TryGetDouble(out var value)
            ? value
            : 0.72;

        return new CopilotResponseDto(
            customerId,
            question,
            answer,
            $"openai-responses:{DateTime.UtcNow:yyyyMMdd}",
            DateTime.UtcNow,
            new StructuredRiskExplanationDto(
                executiveSummary,
                riskLevel,
                confidence,
                ReadSignals(root),
                ReadStringArray(root, "recommendedActions"),
                ReadStringArray(root, "followUpQuestions")),
            knowledge);
    }

    private static IReadOnlyList<CopilotSignalDto> ReadSignals(JsonElement root)
    {
        if (!root.TryGetProperty("topSignals", out var signalsElement) || signalsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var list = new List<CopilotSignalDto>();
        foreach (var item in signalsElement.EnumerateArray())
        {
            var label = item.TryGetProperty("label", out var labelElement) ? labelElement.GetString() ?? "Signal" : "Signal";
            var value = item.TryGetProperty("value", out var valueElement) ? valueElement.GetString() ?? "N/A" : "N/A";
            var impact = item.TryGetProperty("impact", out var impactElement) ? impactElement.GetString() ?? "medium" : "medium";
            list.Add(new CopilotSignalDto(label, value, impact));
        }

        return list;
    }

    private static IReadOnlyList<string> ReadStringArray(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var element) || element.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return element.EnumerateArray()
            .Where(item => item.ValueKind == JsonValueKind.String)
            .Select(item => item.GetString() ?? string.Empty)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .ToList();
    }
}
