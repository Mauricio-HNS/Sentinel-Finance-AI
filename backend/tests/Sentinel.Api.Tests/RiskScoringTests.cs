// ---Made By Destiny7 Softwares---
using Sentinel.Infrastructure;

namespace Sentinel.Api.Tests;

public sealed class RiskScoringTests
{
    [Fact]
    public void Dashboard_ShouldExposeCustomers()
    {
        var service = new DemoSentinelReadService(new DemoDataStore(), new FallbackExplanationGateway());
        var dashboard = service.GetDashboard();

        Assert.True(dashboard.TotalCustomers >= 3);
        Assert.True(dashboard.PortfolioAverageRisk > 0);
    }
}
