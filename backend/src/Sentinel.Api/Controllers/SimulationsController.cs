// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/simulations")]
public sealed class SimulationsController(ISentinelReadService service) : ControllerBase
{
    [HttpPost("run")]
    public ActionResult<PredictionResponse> Run([FromBody] ScenarioSimulationRequest request) => Ok(service.RunSimulation(request));
}
