using Sentinel.Domain;

namespace Sentinel.Infrastructure;

public sealed class DemoDataStore
{
    public IReadOnlyList<Customer> Customers { get; }
    public IReadOnlyList<Payment> Payments { get; }
    public IReadOnlyList<UsageMetric> UsageMetrics { get; }
    public IReadOnlyList<SupportTicket> Tickets { get; }
    public List<Alert> Alerts { get; }
    public IReadOnlyList<RiskAssessment> RiskAssessments { get; }
    public IReadOnlyList<Prediction> Predictions { get; }

    public DemoDataStore()
    {
        var alphaId = Guid.Parse("f0b50a84-cad7-4dcc-9f42-16fd5de2f101");
        var novaId = Guid.Parse("f0b50a84-cad7-4dcc-9f42-16fd5de2f102");
        var orbitId = Guid.Parse("f0b50a84-cad7-4dcc-9f42-16fd5de2f103");

        Customers =
        [
            new Customer { Id = alphaId, Name = "Alpha Capital Partners", Segment = "Enterprise", Country = "United States", Industry = "Fintech", MonthlyRevenue = 182000, CurrentPlan = "Enterprise Pulse", ContractStartDate = new DateOnly(2024, 1, 1), ContractEndDate = new DateOnly(2026, 5, 30) },
            new Customer { Id = novaId, Name = "Nova Retail Cloud", Segment = "Mid-Market", Country = "Brazil", Industry = "Retail Tech", MonthlyRevenue = 76000, CurrentPlan = "Growth Shield", ContractStartDate = new DateOnly(2024, 6, 1), ContractEndDate = new DateOnly(2026, 4, 20) },
            new Customer { Id = orbitId, Name = "Orbit Manufacturing Hub", Segment = "Enterprise", Country = "Germany", Industry = "Industrial SaaS", MonthlyRevenue = 129000, CurrentPlan = "Enterprise Pulse", ContractStartDate = new DateOnly(2023, 9, 1), ContractEndDate = new DateOnly(2026, 6, 15) }
        ];

        Payments =
        [
            new Payment { CustomerId = alphaId, Amount = 182000, DueDate = new DateOnly(2026, 2, 5), PaidDate = new DateOnly(2026, 2, 7), Status = "Paid", DaysLate = 2 },
            new Payment { CustomerId = alphaId, Amount = 182000, DueDate = new DateOnly(2026, 3, 5), PaidDate = null, Status = "Overdue", DaysLate = 12 },
            new Payment { CustomerId = novaId, Amount = 76000, DueDate = new DateOnly(2026, 3, 8), PaidDate = new DateOnly(2026, 3, 8), Status = "Paid", DaysLate = 0 },
            new Payment { CustomerId = orbitId, Amount = 129000, DueDate = new DateOnly(2026, 3, 1), PaidDate = new DateOnly(2026, 3, 3), Status = "Paid", DaysLate = 2 }
        ];

        UsageMetrics =
        [
            new UsageMetric { CustomerId = alphaId, ReferenceMonth = new DateOnly(2026, 1, 1), ActiveUsers = 420, TotalSessions = 11200, FeatureUsageScore = 84, UsageVariationPercent = -8 },
            new UsageMetric { CustomerId = alphaId, ReferenceMonth = new DateOnly(2026, 2, 1), ActiveUsers = 392, TotalSessions = 9800, FeatureUsageScore = 78, UsageVariationPercent = -18 },
            new UsageMetric { CustomerId = alphaId, ReferenceMonth = new DateOnly(2026, 3, 1), ActiveUsers = 350, TotalSessions = 8100, FeatureUsageScore = 71, UsageVariationPercent = -32 },
            new UsageMetric { CustomerId = novaId, ReferenceMonth = new DateOnly(2026, 3, 1), ActiveUsers = 188, TotalSessions = 6400, FeatureUsageScore = 82, UsageVariationPercent = 6 },
            new UsageMetric { CustomerId = orbitId, ReferenceMonth = new DateOnly(2026, 3, 1), ActiveUsers = 265, TotalSessions = 7600, FeatureUsageScore = 80, UsageVariationPercent = -4 }
        ];

        Tickets =
        [
            new SupportTicket { CustomerId = alphaId, Severity = "Critical", Category = "Billing", Status = "Open", ResolutionTimeHours = 22 },
            new SupportTicket { CustomerId = alphaId, Severity = "Critical", Category = "API Reliability", Status = "Resolved", ResolutionTimeHours = 30 },
            new SupportTicket { CustomerId = alphaId, Severity = "Critical", Category = "Access", Status = "Resolved", ResolutionTimeHours = 11 },
            new SupportTicket { CustomerId = alphaId, Severity = "Critical", Category = "Analytics", Status = "Open", ResolutionTimeHours = 0 },
            new SupportTicket { CustomerId = novaId, Severity = "Medium", Category = "Onboarding", Status = "Resolved", ResolutionTimeHours = 5 }
        ];

        RiskAssessments =
        [
            new RiskAssessment { CustomerId = alphaId, OverallRiskScore = 81, ChurnRiskScore = 79, LatePaymentRiskScore = 84, RevenueRiskScore = 76, RiskLevel = RiskLevel.Critical, Summary = "Usage declined 32%, 4 critical tickets this quarter and latest invoice is 12 days overdue." },
            new RiskAssessment { CustomerId = novaId, OverallRiskScore = 28, ChurnRiskScore = 22, LatePaymentRiskScore = 19, RevenueRiskScore = 31, RiskLevel = RiskLevel.Low, Summary = "Healthy usage, no payment delays and stable support sentiment." },
            new RiskAssessment { CustomerId = orbitId, OverallRiskScore = 46, ChurnRiskScore = 41, LatePaymentRiskScore = 38, RevenueRiskScore = 52, RiskLevel = RiskLevel.Medium, Summary = "Stable revenue, mild adoption softness and contract renewal window approaching." }
        ];

        Alerts =
        [
            new Alert { CustomerId = alphaId, Type = "Late Payment", Severity = AlertSeverity.Critical, Title = "12-day overdue invoice", Description = "Latest enterprise invoice remains open for more than 10 days.", IsResolved = false },
            new Alert { CustomerId = alphaId, Type = "Churn", Severity = AlertSeverity.High, Title = "Usage dropped below 75 score", Description = "Platform adoption fell sharply over the last 90 days.", IsResolved = false },
            new Alert { CustomerId = orbitId, Type = "Renewal", Severity = AlertSeverity.Warning, Title = "Renewal window opening", Description = "Contract is within the next 90 days with moderate adoption risk.", IsResolved = false }
        ];

        Predictions =
        [
            new Prediction { CustomerId = alphaId, PredictionType = "Churn", Value = 0.79, Confidence = 0.88 },
            new Prediction { CustomerId = alphaId, PredictionType = "LatePayment", Value = 0.84, Confidence = 0.91 },
            new Prediction { CustomerId = novaId, PredictionType = "Churn", Value = 0.22, Confidence = 0.86 },
            new Prediction { CustomerId = orbitId, PredictionType = "OverallRisk", Value = 0.46, Confidence = 0.81 }
        ];
    }
}
