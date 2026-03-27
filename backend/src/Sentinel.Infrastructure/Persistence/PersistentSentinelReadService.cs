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

            var customer = new Customer
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
            };

            dbContext.Customers.Add(customer);

            var generatedBundle = GenerateImportedSignals(customer);
            dbContext.Contracts.Add(generatedBundle.Contract);
            dbContext.Payments.AddRange(generatedBundle.Payments);
            dbContext.UsageMetrics.AddRange(generatedBundle.UsageMetrics);
            dbContext.SupportTickets.AddRange(generatedBundle.Tickets);
            dbContext.RiskAssessments.Add(generatedBundle.RiskAssessment);
            dbContext.Alerts.AddRange(generatedBundle.Alerts);
            dbContext.Predictions.AddRange(generatedBundle.Predictions);
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

    private static ImportedCustomerBundle GenerateImportedSignals(Customer customer)
    {
        var seed = Math.Abs(customer.Id.GetHashCode());
        var usageDrop = -1 * (8 + seed % 28);
        var daysLate = seed % 4 == 0 ? 0 : 4 + seed % 14;
        var criticalTickets = customer.Segment == "Enterprise" ? 1 + seed % 4 : seed % 2;
        var churnRisk = Math.Clamp(18 + Math.Abs(usageDrop) * 1.2 + criticalTickets * 6, 0, 100);
        var lateRisk = Math.Clamp(14 + daysLate * 4.1 + criticalTickets * 1.8, 0, 100);
        var overallRisk = Math.Clamp((churnRisk * 0.45) + (lateRisk * 0.35) + (Math.Abs(usageDrop) * 0.6), 0, 100);
        var revenueRisk = Math.Clamp((overallRisk * 0.8) + (customer.MonthlyRevenue > 100000 ? 10 : 0), 0, 100);
        var riskLevel = overallRisk >= 75 ? RiskLevel.Critical : overallRisk >= 55 ? RiskLevel.High : overallRisk >= 35 ? RiskLevel.Medium : RiskLevel.Low;

        var contract = new Contract
        {
            CustomerId = customer.Id,
            ContractType = customer.Segment == "Enterprise" ? "Annual Enterprise" : "Growth Plan",
            ContractValue = customer.MonthlyRevenue * 12,
            BillingCycle = "Monthly",
            RenewalDate = customer.ContractEndDate,
            IsAutoRenew = customer.Segment == "Enterprise",
            Status = customer.Status
        };

        var payments = new List<Payment>
        {
            new()
            {
                CustomerId = customer.Id,
                Amount = customer.MonthlyRevenue,
                DueDate = new DateOnly(2026, 1, 5),
                PaidDate = new DateOnly(2026, 1, 6),
                Status = "Paid",
                DaysLate = 1
            },
            new()
            {
                CustomerId = customer.Id,
                Amount = customer.MonthlyRevenue,
                DueDate = new DateOnly(2026, 2, 5),
                PaidDate = daysLate > 0 ? new DateOnly(2026, 2, 5).AddDays(daysLate) : new DateOnly(2026, 2, 5),
                Status = daysLate > 0 ? "Paid" : "Paid",
                DaysLate = daysLate
            },
            new()
            {
                CustomerId = customer.Id,
                Amount = customer.MonthlyRevenue,
                DueDate = new DateOnly(2026, 3, 5),
                PaidDate = daysLate >= 10 ? null : new DateOnly(2026, 3, 7),
                Status = daysLate >= 10 ? "Overdue" : "Paid",
                DaysLate = daysLate >= 10 ? daysLate : 2
            }
        };

        var usageMetrics = new List<UsageMetric>
        {
            new()
            {
                CustomerId = customer.Id,
                ReferenceMonth = new DateOnly(2026, 1, 1),
                ActiveUsers = Math.Max(40, (int)(customer.MonthlyRevenue / 550)),
                TotalSessions = Math.Max(1800, (int)(customer.MonthlyRevenue / 18)),
                FeatureUsageScore = 82,
                UsageVariationPercent = -4
            },
            new()
            {
                CustomerId = customer.Id,
                ReferenceMonth = new DateOnly(2026, 2, 1),
                ActiveUsers = Math.Max(36, (int)(customer.MonthlyRevenue / 600)),
                TotalSessions = Math.Max(1600, (int)(customer.MonthlyRevenue / 19)),
                FeatureUsageScore = 77,
                UsageVariationPercent = usageDrop / 2.0
            },
            new()
            {
                CustomerId = customer.Id,
                ReferenceMonth = new DateOnly(2026, 3, 1),
                ActiveUsers = Math.Max(30, (int)(customer.MonthlyRevenue / 650)),
                TotalSessions = Math.Max(1400, (int)(customer.MonthlyRevenue / 21)),
                FeatureUsageScore = Math.Max(46, 78 - Math.Abs(usageDrop) / 2.0),
                UsageVariationPercent = usageDrop
            }
        };

        var tickets = Enumerable.Range(1, Math.Max(1, criticalTickets + 1))
            .Select(index => new SupportTicket
            {
                CustomerId = customer.Id,
                Severity = index <= criticalTickets ? "Critical" : "Medium",
                Category = index <= criticalTickets ? "Platform Reliability" : "Enablement",
                Status = index <= criticalTickets ? "Open" : "Resolved",
                ResolutionTimeHours = index <= criticalTickets ? 18 + index * 6 : 6
            })
            .ToList();

        var riskAssessment = new RiskAssessment
        {
            CustomerId = customer.Id,
            OverallRiskScore = Math.Round(overallRisk, 1),
            ChurnRiskScore = Math.Round(churnRisk, 1),
            LatePaymentRiskScore = Math.Round(lateRisk, 1),
            RevenueRiskScore = Math.Round(revenueRisk, 1),
            RiskLevel = riskLevel,
            Summary = $"Imported signals indicate usage variation of {usageDrop}%, {criticalTickets} critical tickets, and latest payment lateness of {daysLate} days."
        };

        var alerts = new List<Alert>();
        if (daysLate >= 10)
        {
            alerts.Add(new Alert
            {
                CustomerId = customer.Id,
                Type = "Late Payment",
                Severity = AlertSeverity.Critical,
                Title = "Imported overdue payment risk",
                Description = $"Latest imported payment pattern indicates {daysLate} days late exposure."
            });
        }

        if (usageDrop <= -20)
        {
            alerts.Add(new Alert
            {
                CustomerId = customer.Id,
                Type = "Churn",
                Severity = AlertSeverity.High,
                Title = "Imported adoption decline detected",
                Description = $"Usage metrics show a {usageDrop}% monthly variation."
            });
        }

        if (customer.ContractEndDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber <= 90)
        {
            alerts.Add(new Alert
            {
                CustomerId = customer.Id,
                Type = "Renewal",
                Severity = AlertSeverity.Warning,
                Title = "Imported renewal window approaching",
                Description = "Contract renewal is within the next 90 days."
            });
        }

        var predictions = new List<Prediction>
        {
            new()
            {
                CustomerId = customer.Id,
                PredictionType = "Churn",
                Value = Math.Round(churnRisk / 100, 2),
                Confidence = 0.82,
                ModelVersion = "ingestion-baseline-v1"
            },
            new()
            {
                CustomerId = customer.Id,
                PredictionType = "LatePayment",
                Value = Math.Round(lateRisk / 100, 2),
                Confidence = 0.84,
                ModelVersion = "ingestion-baseline-v1"
            },
            new()
            {
                CustomerId = customer.Id,
                PredictionType = "OverallRisk",
                Value = Math.Round(overallRisk / 100, 2),
                Confidence = 0.8,
                ModelVersion = "ingestion-baseline-v1"
            }
        };

        return new ImportedCustomerBundle(contract, payments, usageMetrics, tickets, riskAssessment, alerts, predictions);
    }

    private sealed record ImportedCustomerBundle(
        Contract Contract,
        IReadOnlyList<Payment> Payments,
        IReadOnlyList<UsageMetric> UsageMetrics,
        IReadOnlyList<SupportTicket> Tickets,
        RiskAssessment RiskAssessment,
        IReadOnlyList<Alert> Alerts,
        IReadOnlyList<Prediction> Predictions);
}
