using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DebugController : ControllerBase
{
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(new { 
            claims,
            hasTenantId = User.FindFirst("tenant_id") != null,
            tenantIdValue = User.FindFirst("tenant_id")?.Value
        });
    }
}
