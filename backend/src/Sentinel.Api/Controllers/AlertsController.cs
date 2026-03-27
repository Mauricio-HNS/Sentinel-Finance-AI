using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public sealed class AlertsController(ISentinelReadService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<AlertDto>> GetAll() => Ok(service.GetAlerts());

    [HttpPatch("{id:guid}/resolve")]
    public IActionResult Resolve(Guid id) => service.ResolveAlert(id) ? NoContent() : NotFound();
}
