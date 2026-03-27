using Sentinel.Domain;

namespace Sentinel.Application;

public sealed record ExecutiveDashboardDto(
    int TotalCustomers,
    double PortfolioAverageRisk,
    int HighRiskCustomers,
    double PredictedChurnPercent,
    double PredictedLatePaymentPercent,
    decimal RevenueAtRisk,
    IReadOnlyList<KpiTrendDto> RiskTrend,
    IReadOnlyList<SegmentRiskDto> SegmentDistribution);

public sealed record KpiTrendDto(string Label, double Value);

public sealed record SegmentRiskDto(string Segment, double RiskScore, decimal RevenueAtRisk);

public sealed record CustomerListItemDto(
    Guid Id,
    string Name,
    string Segment,
    string Industry,
    string Country,
    string CurrentPlan,
    double OverallRiskScore,
    RiskLevel RiskLevel,
    double ChurnRiskScore,
    double LatePaymentRiskScore,
    decimal MonthlyRevenue);

public sealed record CustomerDetailDto(
    Customer Customer,
    RiskAssessment LatestRiskAssessment,
    IReadOnlyList<Payment> Payments,
    IReadOnlyList<UsageMetric> UsageMetrics,
    IReadOnlyList<SupportTicket> Tickets,
    IReadOnlyList<Alert> Alerts,
    IReadOnlyList<Prediction> Predictions);

public sealed record AlertDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string Type,
    AlertSeverity Severity,
    string Title,
    string Description,
    bool IsResolved,
    DateTime CreatedAt);

public sealed record AuthRequest(string Email, string Password);
public sealed record AuthResponse(string Token, string FullName, string Role);
public sealed record CsvUploadResponse(string FileName, int ImportedRows, string Status);
public sealed record ExplanationRequest(Guid CustomerId, string Context);
public sealed record ExplanationResponse(Guid CustomerId, string Explanation);
public sealed record ScenarioSimulationRequest(Guid CustomerId, int DaysLate, double UsageVariationPercent, int CriticalTickets, bool ContractExpiringSoon);
public sealed record PredictionResponse(string PredictionType, double Score, double Confidence, string ModelVersion, string Narrative);
public sealed record CopilotQuestionRequest(Guid CustomerId, string Question);
public sealed record CopilotSignalDto(string Label, string Value, string Impact);
public sealed record KnowledgeChunkDto(string Title, string SourceType, string Path, string Snippet, double Score);
public sealed record StructuredRiskExplanationDto(
    string ExecutiveSummary,
    string RiskLevel,
    double Confidence,
    IReadOnlyList<CopilotSignalDto> TopSignals,
    IReadOnlyList<string> RecommendedActions,
    IReadOnlyList<string> FollowUpQuestions);
public sealed record CopilotResponseDto(
    Guid CustomerId,
    string Question,
    string Answer,
    string Model,
    DateTime GeneratedAt,
    StructuredRiskExplanationDto Analysis,
    IReadOnlyList<KnowledgeChunkDto> Knowledge);
public sealed record EvalRecordDto(
    string EvaluationName,
    string Scenario,
    string ExpectedBehavior,
    string Scorecard,
    string Status,
    string ModelTarget,
    DateTime UpdatedAt);
