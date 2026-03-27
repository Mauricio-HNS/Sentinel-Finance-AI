// ---Made By Destiny7 Softwares---
using Microsoft.EntityFrameworkCore;
using Sentinel.Application;
using Sentinel.Domain;

namespace Sentinel.Infrastructure.Persistence;

public sealed class PersistentSentinelReadService(
    AppDbContext dbContext,
    IExplanationGateway explanationGateway,
    IKnowledgeRetrievalService knowledgeRetrievalService,
    IEvalsTrailService evalsTrailService,
    IAICopilotGateway aiCopilotGateway) : ISentinelReadService
{
    public ExecutiveDashboardDto GetDashboard()
    {
        var risks = dbContext.RiskAssessments.AsNoTracking().ToList();
        var customers = dbContext.Customers.AsNoTracking().ToList();

        if (customers.Count == 0 || risks.Count == 0)
        {
            return new ExecutiveDashboardDto(0, 0, 0, 0, 0, 0, [], []);
        }

        var highRisk = risks.Count(x => x.RiskLevel is RiskLevel.High or RiskLevel.Critical);
        var averageRisk = Math.Round(risks.Average(x => x.OverallRiskScore), 1);
        var revenueAtRisk = customers
            .Join(risks, c => c.Id, r => r.CustomerId, (c, r) => new { c.MonthlyRevenue, r.OverallRiskScore, c.Segment })
            .ToList();

        return new ExecutiveDashboardDto(
            customers.Count,
            averageRisk,
            highRisk,
            Math.Round(risks.Average(x => x.ChurnRiskScore), 1),
            Math.Round(risks.Average(x => x.LatePaymentRiskScore), 1),
            revenueAtRisk.Where(x => x.OverallRiskScore >= 50).Sum(x => x.MonthlyRevenue),
            [
                new KpiTrendDto("Jan", 42),
                new KpiTrendDto("Feb", 48),
                new KpiTrendDto("Mar", 52),
                new KpiTrendDto("Apr", 57)
            ],
            revenueAtRisk
                .GroupBy(x => x.Segment)
                .Select(group => new SegmentRiskDto(
                    group.Key,
                    Math.Round(group.Average(x => x.OverallRiskScore), 1),
                    group.Where(x => x.OverallRiskScore >= 50).Sum(x => x.MonthlyRevenue)))
                .ToList());
    }

    public IReadOnlyList<CustomerListItemDto> GetCustomers() =>
        dbContext.Customers
            .AsNoTracking()
            .Join(
                dbContext.RiskAssessments.AsNoTracking(),
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
        var customer = dbContext.Customers.AsNoTracking().FirstOrDefault(x => x.Id == id);
        var risk = dbContext.RiskAssessments.AsNoTracking().FirstOrDefault(x => x.CustomerId == id);

        return customer is null || risk is null
            ? null
            : new CustomerDetailDto(
                customer,
                risk,
                dbContext.Payments.AsNoTracking().Where(x => x.CustomerId == id).OrderByDescending(x => x.DueDate).ToList(),
                dbContext.UsageMetrics.AsNoTracking().Where(x => x.CustomerId == id).OrderByDescending(x => x.ReferenceMonth).ToList(),
                dbContext.SupportTickets.AsNoTracking().Where(x => x.CustomerId == id).OrderByDescending(x => x.CreatedAt).ToList(),
                dbContext.Alerts.AsNoTracking().Where(x => x.CustomerId == id).OrderByDescending(x => x.CreatedAt).ToList(),
                dbContext.Predictions.AsNoTracking().Where(x => x.CustomerId == id).OrderByDescending(x => x.CreatedAt).ToList());
    }

    public IReadOnlyList<AlertDto> GetAlerts() =>
        dbContext.Alerts
            .AsNoTracking()
            .Join(
                dbContext.Customers.AsNoTracking(),
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
        return detail is null ? [] : knowledgeRetrievalService.Retrieve(customerId, detail.Customer.Name, "contracts tickets support renewal risk");
    }

    public IReadOnlyList<EvalRecordDto> GetRecentEvals() => evalsTrailService.GetRecent();

    public PredictionResponse RecalculateRisk(Guid customerId)
    {
        var detail = GetCustomer(customerId);
        if (detail is null)
        {
            return new PredictionResponse("OverallRisk", 0, 0, "not-found", "Customer not found.");
        }

        var latestUsage = detail.UsageMetrics.FirstOrDefault()?.UsageVariationPercent ?? 0;
        var lateDays = detail.Payments.FirstOrDefault()?.DaysLate ?? 0;
        var criticalTickets = detail.Tickets.Count(x => x.Severity == "Critical");
        var score = Math.Clamp(35 + lateDays * 2.4 + Math.Abs(Math.Min(latestUsage, 0)) * 0.8 + criticalTickets * 4.5, 0, 100);
        var prediction = new PredictionResponse(
            "OverallRisk",
            Math.Round(score, 1),
            0.83,
            "heuristic-v1",
            "Recalculated from payment delays, adoption trend and support severity signals.");

        var existing = dbContext.RiskAssessments.FirstOrDefault(x => x.CustomerId == customerId);
        if (existing is not null)
        {
            dbContext.Entry(existing).CurrentValues.SetValues(new RiskAssessment
            {
                Id = existing.Id,
                CreatedAt = existing.CreatedAt,
                CustomerId = customerId,
                OverallRiskScore = prediction.Score,
                ChurnRiskScore = Math.Clamp(prediction.Score - 2, 0, 100),
                LatePaymentRiskScore = Math.Clamp(lateDays * 7, 0, 100),
                RevenueRiskScore = Math.Clamp(prediction.Score - 5, 0, 100),
                RiskLevel = prediction.Score >= 75 ? RiskLevel.Critical : prediction.Score >= 55 ? RiskLevel.High : prediction.Score >= 35 ? RiskLevel.Medium : RiskLevel.Low,
                Summary = prediction.Narrative
            });
        }

        dbContext.Predictions.Add(new Prediction
        {
            CustomerId = customerId,
            PredictionType = prediction.PredictionType,
            Value = prediction.Score / 100,
            Confidence = prediction.Confidence,
            ModelVersion = prediction.ModelVersion
        });
        dbContext.SaveChanges();

        return prediction;
    }

    public PredictionResponse RunSimulation(ScenarioSimulationRequest request)
    {
        var score = Math.Clamp(24 + request.DaysLate * 2.5 + Math.Abs(Math.Min(request.UsageVariationPercent, 0)) * 0.9 + request.CriticalTickets * 5 + (request.ContractExpiringSoon ? 9 : 0), 0, 100);
        var response = new PredictionResponse(
            "ScenarioSimulation",
            Math.Round(score, 1),
            0.79,
            "simulator-v1",
            "Scenario score combines late payment pressure, adoption loss, support pressure and renewal exposure.");

        dbContext.ScenarioSimulations.Add(new ScenarioSimulation
        {
            CustomerId = request.CustomerId,
            InputJson = System.Text.Json.JsonSerializer.Serialize(request),
            OutputJson = System.Text.Json.JsonSerializer.Serialize(response)
        });
        dbContext.SaveChanges();

        return response;
    }

    public CsvUploadResponse Upload(string fileName, Stream stream)
    {
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();
        var rows = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rows.Length <= 1)
        {
            return new CsvUploadResponse(fileName, 0, "empty");
        }

        var imported = 0;
        foreach (var row in rows.Skip(1))
        {
            var columns = row.Split(',', StringSplitOptions.TrimEntries);
            if (columns.Length < 10 || !Guid.TryParse(columns[0], out var customerId))
            {
                continue;
            }

            if (dbContext.Customers.Any(x => x.Id == customerId))
            {
                continue;
            }

            if (!decimal.TryParse(columns[5], out var monthlyRevenue))
            {
                monthlyRevenue = 0;
            }

            if (!DateOnly.TryParse(columns[7], out var contractStartDate))
            {
                contractStartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            if (!DateOnly.TryParse(columns[8], out var contractEndDate))
            {
                contractEndDate = contractStartDate.AddYears(1);
            }

            dbContext.Customers.Add(new Customer
            {
                Id = customerId,
                Name = columns[1],
                Segment = columns[2],
                Country = columns[3],
                Industry = columns[4],
                MonthlyRevenue = monthlyRevenue,
                CurrentPlan = columns[6],
                ContractStartDate = contractStartDate,
                ContractEndDate = contractEndDate,
                Status = columns[9]
            });

            dbContext.RiskAssessments.Add(new RiskAssessment
            {
                CustomerId = customerId,
                OverallRiskScore = 25,
                ChurnRiskScore = 21,
                LatePaymentRiskScore = 19,
                RevenueRiskScore = 24,
                RiskLevel = RiskLevel.Low,
                Summary = "Imported customer awaiting full signal enrichment."
            });
            imported++;
        }

        dbContext.SaveChanges();
        return new CsvUploadResponse(fileName, imported, "processed");
    }

    public AuthResponse Login(AuthRequest request) => new("sentinel-demo-token", "Alex Morgan", "Executive Analyst");

    public bool ResolveAlert(Guid alertId)
    {
        var alert = dbContext.Alerts.FirstOrDefault(x => x.Id == alertId);
        if (alert is null)
        {
            return false;
        }

        alert.IsResolved = true;
        dbContext.SaveChanges();
        return true;
    }
}
