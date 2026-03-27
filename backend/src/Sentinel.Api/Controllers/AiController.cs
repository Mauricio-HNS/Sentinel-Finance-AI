using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/ai/explanations")]
public sealed class AiController(ISentinelReadService service) : ControllerBase
{
    [HttpPost("generate")]
    public ActionResult<ExplanationResponse> Generate([FromBody] ExplanationRequest request) => Ok(service.GenerateExplanation(request));
}
