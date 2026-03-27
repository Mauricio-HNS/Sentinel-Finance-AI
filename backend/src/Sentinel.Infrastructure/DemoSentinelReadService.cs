using Sentinel.Application;
using Sentinel.Domain;

namespace Sentinel.Infrastructure;

public sealed class DemoSentinelReadService(
    DemoDataStore store,
    IExplanationGateway explanationGateway,
    IKnowledgeRetrievalService knowledgeRetrievalService,
    IEvalsTrailService evalsTrailService,
    IAICopilotGateway aiCopilotGateway) : ISentinelReadService
{
    public ExecutiveDashboardDto GetDashboard()
    {
        var highRisk = store.RiskAssessments.Count(x => x.RiskLevel is RiskLevel.High or RiskLevel.Critical);
        var averageRisk = Math.Round(store.RiskAssessments.Average(x => x.OverallRiskScore), 1);
        var revenueAtRisk = store.Customers
            .Join(store.RiskAssessments, c => c.Id, r => r.CustomerId, (c, r) => new { c.MonthlyRevenue, r.OverallRiskScore })
            .Where(x => x.OverallRiskScore >= 50)
            .Sum(x => x.MonthlyRevenue);

        return new ExecutiveDashboardDto(
            store.Customers.Count,
            averageRisk,
            highRisk,
            34.0,
            29.0,
            revenueAtRisk,
            [
                new KpiTrendDto("Jan", 42),
                new KpiTrendDto("Feb", 48),
                new KpiTrendDto("Mar", 52),
                new KpiTrendDto("Apr", 57)
            ],
            [
                new SegmentRiskDto("Enterprise", 63, 311000),
                new SegmentRiskDto("Mid-Market", 28, 22000),
                new SegmentRiskDto("SMB", 19, 8000)
            ]);
    }

    public IReadOnlyList<CustomerListItemDto> GetCustomers() =>
        store.Customers.Join(
            store.RiskAssessments,
            customer => customer.Id,
            risk => risk.CustomerId,
            (customer, risk) => new CustomerListItemDto(
                customer.Id,
                customer.Name,
                customer.Segment,
                customer.Industry,
                customer.Country,
                customer.CurrentPlan,
                risk.OverallRiskScore,
                risk.RiskLevel,
                risk.ChurnRiskScore,
                risk.LatePaymentRiskScore,
                customer.MonthlyRevenue))
        .OrderByDescending(x => x.OverallRiskScore)
        .ToList();

    public CustomerDetailDto? GetCustomer(Guid id)
    {
        var customer = store.Customers.FirstOrDefault(x => x.Id == id);
        var risk = store.RiskAssessments.FirstOrDefault(x => x.CustomerId == id);

        return customer is null || risk is null
            ? null
            : new CustomerDetailDto(
                customer,
                risk,
                store.Payments.Where(x => x.CustomerId == id).ToList(),
                store.UsageMetrics.Where(x => x.CustomerId == id).ToList(),
                store.Tickets.Where(x => x.CustomerId == id).ToList(),
                store.Alerts.Where(x => x.CustomerId == id).ToList(),
                store.Predictions.Where(x => x.CustomerId == id).ToList());
    }

    public IReadOnlyList<AlertDto> GetAlerts() =>
        store.Alerts.Join(
            store.Customers,
            alert => alert.CustomerId,
            customer => customer.Id,
            (alert, customer) => new AlertDto(
                alert.Id,
                alert.CustomerId,
                customer.Name,
                alert.Type,
                alert.Severity,
                alert.Title,
                alert.Description,
                alert.IsResolved,
                alert.CreatedAt))
        .OrderByDescending(x => x.CreatedAt)
        .ToList();

    public ExplanationResponse GenerateExplanation(ExplanationRequest request)
    {
        var generated = explanationGateway.GenerateAsync(request.Context).GetAwaiter().GetResult();
        return new ExplanationResponse(request.CustomerId, generated);
    }

    public CopilotResponseDto GetCopilotBriefing(Guid customerId, string? question = null)
    {
        var detail = GetCustomer(customerId);
        if (detail is null)
        {
            return new CopilotResponseDto(
                customerId,
                question ?? "What is driving current risk?",
                "Customer not found.",
                "fallback-structured-copilot",
                DateTime.UtcNow,
                new StructuredRiskExplanationDto("Customer not found.", "Unknown", 0, [], [], []),
                []);
        }

        var prompt = question ?? "What is driving current risk and what should leadership do next?";
        var knowledge = knowledgeRetrievalService.Retrieve(customerId, detail.Customer.Name, prompt);
        return aiCopilotGateway.GenerateAsync(detail, prompt, knowledge).GetAwaiter().GetResult();
    }

    public IReadOnlyList<KnowledgeChunkDto> GetKnowledgeBase(Guid customerId)
    {
        var detail = GetCustomer(customerId);
        return detail is null
            ? []
            : knowledgeRetrievalService.Retrieve(customerId, detail.Customer.Name, "contracts tickets support renewal risk");
    }

    public IReadOnlyList<EvalRecordDto> GetRecentEvals() => evalsTrailService.GetRecent();

    public PredictionResponse RecalculateRisk(Guid customerId)
    {
        var detail = GetCustomer(customerId);
        if (detail is null)
        {
            return new PredictionResponse("OverallRisk", 0, 0, "not-found", "Customer not found.");
        }

        var latestUsage = detail.UsageMetrics.OrderByDescending(x => x.ReferenceMonth).FirstOrDefault()?.UsageVariationPercent ?? 0;
        var lateDays = detail.Payments.OrderByDescending(x => x.DueDate).FirstOrDefault()?.DaysLate ?? 0;
        var criticalTickets = detail.Tickets.Count(x => x.Severity == "Critical");
        var score = Math.Clamp(35 + lateDays * 2.4 + Math.Abs(Math.Min(latestUsage, 0)) * 0.8 + criticalTickets * 4.5, 0, 100);

        return new PredictionResponse(
            "OverallRisk",
            Math.Round(score, 1),
            0.83,
            "heuristic-v1",
            "Recalculated from payment delays, adoption trend and support severity signals.");
    }

    public PredictionResponse RunSimulation(ScenarioSimulationRequest request)
    {
        var score = Math.Clamp(24 + request.DaysLate * 2.5 + Math.Abs(Math.Min(request.UsageVariationPercent, 0)) * 0.9 + request.CriticalTickets * 5 + (request.ContractExpiringSoon ? 9 : 0), 0, 100);
        return new PredictionResponse(
            "ScenarioSimulation",
            Math.Round(score, 1),
            0.79,
            "simulator-v1",
            "Scenario score combines late payment pressure, adoption loss, support pressure and renewal exposure.");
    }

    public CsvUploadResponse Upload(string fileName, Stream stream)
    {
        using var reader = new StreamReader(stream);
        var rows = reader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries).Length - 1;
        return new CsvUploadResponse(fileName, Math.Max(rows, 0), "processed");
    }

    public AuthResponse Login(AuthRequest request) => new("sentinel-demo-token", "Alex Morgan", "Executive Analyst");

    public bool ResolveAlert(Guid alertId)
    {
        var alert = store.Alerts.FirstOrDefault(x => x.Id == alertId);
        if (alert is null)
        {
            return false;
        }

        alert.IsResolved = true;
        return true;
    }
}
