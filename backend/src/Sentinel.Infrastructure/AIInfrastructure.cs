// ---Made By Destiny7 Softwares---
using System.Net.Http.Headers;
using System.Text.Json;
using Sentinel.Application;
using Sentinel.Domain;

namespace Sentinel.Infrastructure;

public sealed class FileKnowledgeRetrievalService : IKnowledgeRetrievalService
{
    public string Mode => "local-markdown-retrieval";

    private readonly string _knowledgeRoot;

    public FileKnowledgeRetrievalService()
    {
        _knowledgeRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "knowledge"));
    }

    public IReadOnlyList<KnowledgeChunkDto> Retrieve(Guid customerId, string customerName, string question, int take = 4)
    {
        if (!Directory.Exists(_knowledgeRoot))
        {
            return [];
        }

        var keywords = $"{customerName} {question}".Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .Distinct()
            .ToArray();

        return Directory
            .EnumerateFiles(_knowledgeRoot, "*.md", SearchOption.AllDirectories)
            .Select(path =>
            {
                var content = File.ReadAllText(path);
                var score = keywords.Count(keyword => content.Contains(keyword, StringComparison.OrdinalIgnoreCase));
                var sourceType = path.Contains($"{Path.DirectorySeparatorChar}contracts{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                    ? "contract"
                    : path.Contains($"{Path.DirectorySeparatorChar}tickets{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
                        ? "ticket"
                        : "playbook";

                var snippet = content.Length > 220 ? $"{content[..220].Trim()}..." : content.Trim();
                return new KnowledgeChunkDto(Path.GetFileNameWithoutExtension(path), sourceType, path, snippet, score);
            })
            .Where(x => x.Score > 0 || x.SourceType is "contract" or "ticket")
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.Title)
            .Take(take)
            .ToList();
    }
}

public sealed class FileEvalsTrailService : IEvalsTrailService
{
    public string Mode => "local-eval-trail";

    private readonly string _evalsPath;

    public FileEvalsTrailService()
    {
        _evalsPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "evals", "risk-copilot-evals.json"));
    }

    public IReadOnlyList<EvalRecordDto> GetRecent()
    {
        if (!File.Exists(_evalsPath))
        {
            return [];
        }

        var json = File.ReadAllText(_evalsPath);
        var records = JsonSerializer.Deserialize<List<EvalRecordDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return records ?? [];
    }

    public Task<EvalRunResponseDto> RunRiskCopilotEvalAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new EvalRunResponseDto(
            Mode,
            "preview-only",
            "risk-copilot-local-suite",
            "Local demo mode is active. Configure an OpenAI eval id to trigger a remote eval run.",
            DateTime.UtcNow,
            null,
            null));
    }
}

public sealed class FallbackAICopilotGateway : IAICopilotGateway
{
    public string Mode => "deterministic-fallback-copilot";

    public Task<CopilotResponseDto> GenerateAsync(
        CustomerDetailDto detail,
        string question,
        IReadOnlyList<KnowledgeChunkDto> knowledge,
        CancellationToken cancellationToken = default)
    {
        var criticalTickets = detail.Tickets.Count(x => x.Severity == "Critical");
        var latestUsage = detail.UsageMetrics.OrderByDescending(x => x.ReferenceMonth).FirstOrDefault()?.UsageVariationPercent ?? 0;
        var latestLate = detail.Payments.OrderByDescending(x => x.DueDate).FirstOrDefault()?.DaysLate ?? 0;

        var analysis = new StructuredRiskExplanationDto(
            ExecutiveSummary: $"{detail.Customer.Name} currently sits in a {detail.LatestRiskAssessment.RiskLevel} risk band because billing pressure, adoption decline, and service instability are reinforcing each other.",
            RiskLevel: detail.LatestRiskAssessment.RiskLevel.ToString(),
            Confidence: 0.84,
            TopSignals:
            [
                new CopilotSignalDto("Payment delay", $"{latestLate} days", latestLate >= 10 ? "high" : "medium"),
                new CopilotSignalDto("Usage variation", $"{latestUsage}%", latestUsage <= -20 ? "high" : "medium"),
                new CopilotSignalDto("Critical tickets", criticalTickets.ToString(), criticalTickets >= 3 ? "high" : "medium")
            ],
            RecommendedActions:
            [
                "Launch coordinated collections and customer success outreach within 24 hours.",
                "Review unresolved critical incidents with platform engineering and customer stakeholders.",
                "Prepare a renewal containment plan tied to measurable adoption recovery."
            ],
            FollowUpQuestions:
            [
                "Which support themes are most correlated with recent usage decline?",
                "Should we trigger an executive renewal review this week?",
                "What scenario reduces revenue risk fastest without discounting?"
            ]);

        var answer = $"{analysis.ExecutiveSummary} Top signals include {latestLate} days late on the latest invoice, {latestUsage}% usage variation, and {criticalTickets} critical tickets. Recommended next move: coordinate collections, customer success, and engineering as one account recovery motion.";

        return Task.FromResult(new CopilotResponseDto(
            detail.Customer.Id,
            question,
            answer,
            "fallback-structured-copilot",
            DateTime.UtcNow,
            analysis,
            knowledge));
    }
}

public sealed class OpenAiFileSearchKnowledgeRetrievalService(
    HttpClient httpClient,
    OpenAIOptions options,
    FileKnowledgeRetrievalService fallbackRetrievalService) : IKnowledgeRetrievalService
{
    public string Mode => HasRemoteRetrieval() ? "openai-file-search" : fallbackRetrievalService.Mode;

    public IReadOnlyList<KnowledgeChunkDto> Retrieve(Guid customerId, string customerName, string question, int take = 4)
    {
        if (!HasRemoteRetrieval())
        {
            return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
        }

        try
        {
            var payload = new
            {
                model = options.Model,
                input = new object[]
                {
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text = $"Find the strongest evidence for customer {customerName} about this question: {question}"
                            }
                        }
                    }
                },
                tools = new object[]
                {
                    new
                    {
                        type = "file_search",
                        vector_store_ids = new[] { options.VectorStoreId },
                        max_num_results = take
                    }
                },
                text = new
                {
                    format = new
                    {
                        type = "json_schema",
                        name = "knowledge_chunks",
                        strict = true,
                        schema = new
                        {
                            type = "object",
                            additionalProperties = false,
                            properties = new
                            {
                                chunks = new
                                {
                                    type = "array",
                                    items = new
                                    {
                                        type = "object",
                                        additionalProperties = false,
                                        properties = new
                                        {
                                            title = new { type = "string" },
                                            sourceType = new { type = "string" },
                                            path = new { type = "string" },
                                            snippet = new { type = "string" },
                                            score = new { type = "number" }
                                        },
                                        required = new[] { "title", "sourceType", "path", "snippet", "score" }
                                    }
                                }
                            },
                            required = new[] { "chunks" }
                        }
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "responses");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = httpClient.Send(request);
            if (!response.IsSuccessStatusCode)
            {
                return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
            }

            var raw = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
            }

            using var document = JsonDocument.Parse(raw);
            var text = ExtractResponseText(document.RootElement);
            if (string.IsNullOrWhiteSpace(text))
            {
                return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
            }

            using var parsed = JsonDocument.Parse(text);
            if (!parsed.RootElement.TryGetProperty("chunks", out var chunks) || chunks.ValueKind != JsonValueKind.Array)
            {
                return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
            }

            var items = new List<KnowledgeChunkDto>();
            foreach (var item in chunks.EnumerateArray())
            {
                items.Add(new KnowledgeChunkDto(
                    item.TryGetProperty("title", out var title) ? title.GetString() ?? "Knowledge" : "Knowledge",
                    item.TryGetProperty("sourceType", out var sourceType) ? sourceType.GetString() ?? "retrieval" : "retrieval",
                    item.TryGetProperty("path", out var path) ? path.GetString() ?? "openai-file-search" : "openai-file-search",
                    item.TryGetProperty("snippet", out var snippet) ? snippet.GetString() ?? string.Empty : string.Empty,
                    item.TryGetProperty("score", out var score) && score.TryGetDouble(out var value) ? value : 0.75));
            }

            return items.Count == 0 ? fallbackRetrievalService.Retrieve(customerId, customerName, question, take) : items;
        }
        catch
        {
            return fallbackRetrievalService.Retrieve(customerId, customerName, question, take);
        }
    }

    private bool HasRemoteRetrieval() =>
        !string.IsNullOrWhiteSpace(options.ApiKey) && !string.IsNullOrWhiteSpace(options.VectorStoreId);

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
}

public sealed class OpenAiEvalsTrailService(
    HttpClient httpClient,
    OpenAIOptions options,
    FileEvalsTrailService fallbackEvalsTrailService) : IEvalsTrailService
{
    public string Mode => HasEvalRunConfigured() ? "openai-evals" : fallbackEvalsTrailService.Mode;

    public IReadOnlyList<EvalRecordDto> GetRecent() => fallbackEvalsTrailService.GetRecent();

    public async Task<EvalRunResponseDto> RunRiskCopilotEvalAsync(CancellationToken cancellationToken = default)
    {
        if (!HasEvalRunConfigured())
        {
            return await fallbackEvalsTrailService.RunRiskCopilotEvalAsync(cancellationToken);
        }

        var payload = new
        {
            name = $"sentinel-risk-copilot-smoke-{DateTime.UtcNow:yyyyMMddHHmmss}",
            metadata = new
            {
                project = "sentinel-finance-ai",
                suite = "risk-copilot",
                environment = "portfolio-demo"
            },
            data_source = new
            {
                type = "jsonl",
                source = new
                {
                    type = "file_content",
                    content = """
                    {"item":{"customer_name":"Alpha Capital","question":"What is driving current risk and what should leadership do next?"}}
                    {"item":{"customer_name":"Gamma Inc.","question":"What evidence suggests churn exposure is increasing?"}}
                    """
                }
            }
        };

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"evals/{options.RiskCopilotEvalId}/runs");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return await fallbackEvalsTrailService.RunRiskCopilotEvalAsync(cancellationToken);
            }

            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return await fallbackEvalsTrailService.RunRiskCopilotEvalAsync(cancellationToken);
            }

            using var document = JsonDocument.Parse(raw);
            var runId = document.RootElement.TryGetProperty("id", out var idElement) ? idElement.GetString() : null;
            var reportUrl = document.RootElement.TryGetProperty("report_url", out var reportUrlElement) ? reportUrlElement.GetString() : null;
            var status = document.RootElement.TryGetProperty("status", out var statusElement) ? statusElement.GetString() ?? "queued" : "queued";

            return new EvalRunResponseDto(
                Mode,
                status,
                options.RiskCopilotEvalId!,
                "Remote OpenAI eval run requested for the Sentinel risk copilot smoke suite.",
                DateTime.UtcNow,
                runId,
                reportUrl);
        }
        catch
        {
            return await fallbackEvalsTrailService.RunRiskCopilotEvalAsync(cancellationToken);
        }
    }

    private bool HasEvalRunConfigured() =>
        !string.IsNullOrWhiteSpace(options.ApiKey) && !string.IsNullOrWhiteSpace(options.RiskCopilotEvalId);
}
