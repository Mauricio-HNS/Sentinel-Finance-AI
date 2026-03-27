// ---Made By Destiny7 Softwares---
using Microsoft.EntityFrameworkCore;
using Sentinel.Domain;

namespace Sentinel.Infrastructure.Persistence;

public sealed class AppDbSeeder(AppDbContext dbContext, DemoDataStore demoDataStore)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Customers.AnyAsync(cancellationToken))
        {
            return;
        }

        await dbContext.Customers.AddRangeAsync(demoDataStore.Customers, cancellationToken);
        await dbContext.Contracts.AddRangeAsync(BuildContracts(), cancellationToken);
        await dbContext.Payments.AddRangeAsync(demoDataStore.Payments, cancellationToken);
        await dbContext.UsageMetrics.AddRangeAsync(demoDataStore.UsageMetrics, cancellationToken);
        await dbContext.SupportTickets.AddRangeAsync(demoDataStore.Tickets, cancellationToken);
        await dbContext.RiskAssessments.AddRangeAsync(demoDataStore.RiskAssessments, cancellationToken);
        await dbContext.Alerts.AddRangeAsync(demoDataStore.Alerts, cancellationToken);
        await dbContext.Predictions.AddRangeAsync(demoDataStore.Predictions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private IEnumerable<Contract> BuildContracts() =>
        demoDataStore.Customers.Select(customer => new Contract
        {
            CustomerId = customer.Id,
            ContractType = customer.Segment == "Enterprise" ? "Annual Enterprise" : "Growth Plan",
            ContractValue = customer.MonthlyRevenue * 12,
            BillingCycle = "Monthly",
            RenewalDate = customer.ContractEndDate,
            IsAutoRenew = customer.Segment == "Enterprise",
            Status = customer.Status
        });
}
