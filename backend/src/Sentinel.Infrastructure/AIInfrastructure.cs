using System.Text.Json;
using Sentinel.Application;
using Sentinel.Domain;

namespace Sentinel.Infrastructure;

public sealed class FileKnowledgeRetrievalService : IKnowledgeRetrievalService
{
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
}

public sealed class FallbackAICopilotGateway : IAICopilotGateway
{
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
