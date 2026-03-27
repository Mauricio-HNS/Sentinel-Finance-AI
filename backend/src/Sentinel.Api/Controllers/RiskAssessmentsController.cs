// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/risk-assessments")]
public sealed class RiskAssessmentsController(ISentinelReadService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<ExecutiveDashboardDto> GetDashboardSummary() => Ok(service.GetDashboard());

    [HttpGet("customer/{customerId:guid}")]
    public ActionResult<CustomerDetailDto> GetCustomerRisk(Guid customerId)
    {
        var customer = service.GetCustomer(customerId);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost("recalculate/{customerId:guid}")]
    public ActionResult<PredictionResponse> Recalculate(Guid customerId) => Ok(service.RecalculateRisk(customerId));
}
