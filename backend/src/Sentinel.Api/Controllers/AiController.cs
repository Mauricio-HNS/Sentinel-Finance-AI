// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/ai")]
public sealed class AiController(ISentinelReadService service) : ControllerBase
{
    [HttpPost("explanations/generate")]
    public ActionResult<ExplanationResponse> Generate([FromBody] ExplanationRequest request) => Ok(service.GenerateExplanation(request));

    [HttpGet("copilot/customer/{customerId:guid}")]
    public ActionResult<CopilotResponseDto> GetCopilotBriefing(Guid customerId, [FromQuery] string? question = null) =>
        Ok(service.GetCopilotBriefing(customerId, question));

    [HttpGet("knowledge/customer/{customerId:guid}")]
    public ActionResult<IReadOnlyList<KnowledgeChunkDto>> GetKnowledge(Guid customerId) => Ok(service.GetKnowledgeBase(customerId));

    [HttpGet("status")]
    public ActionResult<AiPlatformStatusDto> GetStatus() => Ok(service.GetAiStatus());

    [HttpGet("evals/recent")]
    public ActionResult<IReadOnlyList<EvalRecordDto>> GetRecentEvals() => Ok(service.GetRecentEvals());

    [HttpPost("evals/run-risk-copilot")]
    public ActionResult<EvalRunResponseDto> RunRiskCopilotEval() => Ok(service.RunRiskCopilotEval());
}
