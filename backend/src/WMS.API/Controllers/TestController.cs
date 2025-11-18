using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly EndpointDataSource _endpointDataSource;

    public TestController(EndpointDataSource endpointDataSource)
    {
        _endpointDataSource = endpointDataSource;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            message = "Test endpoint is working!", 
            timestamp = DateTime.UtcNow,
            controllerName = "TestController",
            route = "api/v1/Test"
        });
    }

    [HttpGet("routes")]
    public IActionResult GetAllRoutes()
    {
        var endpoints = _endpointDataSource.Endpoints
            .OfType<RouteEndpoint>()
            .Select(e => new
            {
                Pattern = e.RoutePattern.RawText,
                Methods = e.Metadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>().SelectMany(m => m.HttpMethods).ToArray().Length > 0
                    ? e.Metadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>().SelectMany(m => m.HttpMethods).ToArray()
                    : new[] { "ANY" },
                Name = e.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.EndpointNameMetadata>()?.EndpointName
            })
            .OrderBy(e => e.Pattern)
            .ToList();

        return Ok(new
        {
            totalEndpoints = endpoints.Count,
            endpoints = endpoints,
            searchFor = new
            {
                roles = endpoints.Any(e => e.Pattern?.Contains("Roles") == true),
                permissions = endpoints.Any(e => e.Pattern?.Contains("Permissions") == true),
                systemRoles = endpoints.Any(e => e.Pattern?.Contains("SystemRoles") == true)
            }
        });
    }

    [HttpGet("roles-check")]
    public IActionResult RolesCheck()
    {
        return Ok(new { 
            message = "If you see this, routing is working. Check if RolesController exists in same namespace.",
            expectedRoute = "api/v1/Roles"
        });
    }
}
