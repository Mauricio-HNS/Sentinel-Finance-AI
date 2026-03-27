// ---Made By Destiny7 Softwares---
using Microsoft.EntityFrameworkCore;
using Sentinel.Domain;

namespace Sentinel.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<UsageMetric> UsageMetrics => Set<UsageMetric>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<RiskAssessment> RiskAssessments => Set<RiskAssessment>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Prediction> Predictions => Set<Prediction>();
    public DbSet<ScenarioSimulation> ScenarioSimulations => Set<ScenarioSimulation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasKey(x => x.Id);
        modelBuilder.Entity<Contract>().HasKey(x => x.Id);
        modelBuilder.Entity<Payment>().HasKey(x => x.Id);
        modelBuilder.Entity<UsageMetric>().HasKey(x => x.Id);
        modelBuilder.Entity<SupportTicket>().HasKey(x => x.Id);
        modelBuilder.Entity<RiskAssessment>().HasKey(x => x.Id);
        modelBuilder.Entity<Alert>().HasKey(x => x.Id);
        modelBuilder.Entity<Prediction>().HasKey(x => x.Id);
        modelBuilder.Entity<ScenarioSimulation>().HasKey(x => x.Id);

        modelBuilder.Entity<Customer>().Property(x => x.MonthlyRevenue).HasPrecision(18, 2);
        modelBuilder.Entity<Contract>().Property(x => x.ContractValue).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>().Property(x => x.Amount).HasPrecision(18, 2);
    }
}
