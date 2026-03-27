// ---Made By Destiny7 Softwares---
using Sentinel.Domain.Common;

namespace Sentinel.Domain;

public sealed class Customer : BaseEntity
{
    public string Name { get; init; } = string.Empty;
    public string Segment { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Industry { get; init; } = string.Empty;
    public decimal MonthlyRevenue { get; init; }
    public string CurrentPlan { get; init; } = string.Empty;
    public DateOnly ContractStartDate { get; init; }
    public DateOnly ContractEndDate { get; init; }
    public string Status { get; init; } = "Active";
}

public sealed class Contract : BaseEntity
{
    public Guid CustomerId { get; init; }
    public string ContractType { get; init; } = string.Empty;
    public decimal ContractValue { get; init; }
    public string BillingCycle { get; init; } = "Monthly";
    public DateOnly RenewalDate { get; init; }
    public bool IsAutoRenew { get; init; }
    public string Status { get; init; } = "Active";
}

public sealed class Payment : BaseEntity
{
    public Guid CustomerId { get; init; }
    public decimal Amount { get; init; }
    public DateOnly DueDate { get; init; }
    public DateOnly? PaidDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int DaysLate { get; init; }
}

public sealed class UsageMetric : BaseEntity
{
    public Guid CustomerId { get; init; }
    public DateOnly ReferenceMonth { get; init; }
    public int ActiveUsers { get; init; }
    public int TotalSessions { get; init; }
    public double FeatureUsageScore { get; init; }
    public double UsageVariationPercent { get; init; }
}

public sealed class SupportTicket : BaseEntity
{
    public Guid CustomerId { get; init; }
    public string Severity { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public double ResolutionTimeHours { get; init; }
}

public sealed class RiskAssessment : BaseEntity
{
    public Guid CustomerId { get; init; }
    public double OverallRiskScore { get; init; }
    public double ChurnRiskScore { get; init; }
    public double LatePaymentRiskScore { get; init; }
    public double RevenueRiskScore { get; init; }
    public RiskLevel RiskLevel { get; init; }
    public string Summary { get; init; } = string.Empty;
}

public sealed class Alert : BaseEntity
{
    public Guid CustomerId { get; init; }
    public string Type { get; init; } = string.Empty;
    public AlertSeverity Severity { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsResolved { get; set; }
}

public sealed class Prediction : BaseEntity
{
    public Guid CustomerId { get; init; }
    public string PredictionType { get; init; } = string.Empty;
    public double Value { get; init; }
    public double Confidence { get; init; }
    public string ModelVersion { get; init; } = "heuristic-v1";
}

public sealed class ScenarioSimulation : BaseEntity
{
    public Guid CustomerId { get; init; }
    public string InputJson { get; init; } = "{}";
    public string OutputJson { get; init; } = "{}";
}
