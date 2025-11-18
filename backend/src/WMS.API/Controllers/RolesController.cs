using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Infrastructure.Persistence;

namespace WMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        ApplicationDbContext context,
        ILogger<RolesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<RoleResponse>>> GetAll()
    {
        try
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    UserCount = r.UserRoles.Count,
                    Permissions = r.RolePermissions.Select(rp => new PermissionResponse
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description,
                        Module = rp.Permission.Module,
                        CreatedAt = rp.Permission.CreatedAt
                    }).ToList(),
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching roles");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoleResponse>> GetById(Guid id)
    {
        try
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .Where(r => r.Id == id && !r.IsDeleted)
                .Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    UserCount = r.UserRoles.Count,
                    Permissions = r.RolePermissions.Select(rp => new PermissionResponse
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description,
                        Module = rp.Permission.Module,
                        CreatedAt = rp.Permission.CreatedAt
                    }).ToList(),
                    CreatedAt = r.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoleResponse>> Create([FromBody] CreateRoleRequest request)
    {
        try
        {
            // Verificar se já existe
            var exists = await _context.Roles
                .AnyAsync(r => r.Name == request.Name && !r.IsDeleted);

            if (exists)
            {
                return BadRequest(new { message = "Já existe um role com este nome" });
            }

            var role = new Role(
                name: request.Name,
                description: request.Description,
                isSystemRole: false,
                tenantId: null
            );

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var response = new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole,
                UserCount = 0,
                Permissions = new List<PermissionResponse>(),
                CreatedAt = role.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = role.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<RoleResponse>> Update(Guid id, [FromBody] UpdateRoleRequest request)
    {
        try
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null || role.IsDeleted)
            {
                return NotFound();
            }

            // Proteger roles de sistema
            if (role.IsSystemRole)
            {
                return BadRequest(new { message = "Não é possível editar roles de sistema" });
            }

            // Verificar nome duplicado
            var exists = await _context.Roles
                .AnyAsync(r => r.Name == request.Name && r.Id != id && !r.IsDeleted);

            if (exists)
            {
                return BadRequest(new { message = "Já existe um role com este nome" });
            }

            role.Update(request.Name, request.Description);
            role.UpdateTimestamp("system");

            await _context.SaveChangesAsync();

            // Retornar role atualizado
            var updated = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .Where(r => r.Id == id)
                .Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsSystemRole = r.IsSystemRole,
                    UserCount = r.UserRoles.Count,
                    Permissions = r.RolePermissions.Select(rp => new PermissionResponse
                    {
                        Id = rp.Permission.Id,
                        Name = rp.Permission.Name,
                        Resource = rp.Permission.Resource,
                        Action = rp.Permission.Action,
                        Description = rp.Permission.Description,
                        Module = rp.Permission.Module,
                        CreatedAt = rp.Permission.CreatedAt
                    }).ToList(),
                    CreatedAt = r.CreatedAt
                })
                .FirstAsync();

            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
            {
                return NotFound();
            }

            // Proteger roles de sistema
            if (role.IsSystemRole)
            {
                return BadRequest(new { message = "Não é possível deletar roles de sistema" });
            }

            // Verificar se tem usuários
            if (role.UserRoles.Any())
            {
                return BadRequest(new { message = "Não é possível deletar role com usuários atribuídos" });
            }

            // Soft delete
            role.MarkAsDeleted("system");

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("{id}/permissions")]
    [Authorize]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsRequest request)
    {
        try
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
            {
                return NotFound();
            }

            // Remover permissões antigas
            _context.RolePermissions.RemoveRange(role.RolePermissions);

            // Adicionar novas permissões
            foreach (var permissionId in request.PermissionIds)
            {
                var permission = await _context.Permissions.FindAsync(permissionId);
                if (permission != null && !permission.IsDeleted)
                {
                    _context.RolePermissions.Add(new RolePermission(
                        roleId: role.Id,
                        permissionId: permissionId,
                        assignedBy: "system"
                    ));
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role {RoleId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

// DTOs
public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AssignPermissionsRequest
{
    public List<Guid> PermissionIds { get; set; } = new();
}
