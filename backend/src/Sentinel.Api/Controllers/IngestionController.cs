// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/ingestion")]
public sealed class IngestionController(ISentinelReadService service) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<CsvUploadResponse>> Upload(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        return Ok(service.Upload(file.FileName, stream));
    }
}
