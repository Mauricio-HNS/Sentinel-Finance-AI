// ---Made By Destiny7 Softwares---
using Microsoft.AspNetCore.Mvc;
using Sentinel.Application;

namespace Sentinel.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISentinelReadService service) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<AuthResponse> Login([FromBody] AuthRequest request) => Ok(service.Login(request));
}
