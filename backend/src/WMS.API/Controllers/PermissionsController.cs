using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs.Responses;
using WMS.Infrastructure.Persistence;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(ApplicationDbContext context, ILogger<PermissionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all permissions");
        
        var permissions = await _context.Permissions
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync();

        var response = permissions.Select(p => new PermissionResponse
        {
            Id = p.Id,
            Name = p.Name,
            Resource = p.Resource,
            Action = p.Action,
            Description = p.Description,
            Module = p.Module,
            CreatedAt = p.CreatedAt
        }).ToList();

        return Ok(response);
    }
}
