using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS.Application.DTOs.Requests;
using WMS.Application.Services;
using WMS.Domain.Entities;
using WMS.Infrastructure.Persistence;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints para gerenciamento de usuários
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _context;

    public UsersController(
        IUserService userService, 
        ILogger<UsersController> logger,
        ApplicationDbContext context)
    {
        _userService = userService;
        _logger = logger;
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user: {UserId}", id);

        var result = await _userService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("User not found: {UserId}", id);
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém lista de todos os usuários
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de usuários</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all users");

        var result = await _userService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get users: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo usuário
    /// </summary>
    /// <param name="request">Dados do novo usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new user: {Username}", request.Username);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _userService.CreateAsync(request, createdBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create user: {Username}. Reason: {Error}", request.Username, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("User created successfully: {Username}", request.Username);
        return CreatedAtAction(nameof(GetUserById), new { id = result.Value?.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="request">Dados atualizados do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user: {UserId} with data: {@Request}", id, request);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid ModelState: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var updatedBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _userService.UpdateAsync(id, request, updatedBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update user: {UserId}. Reason: {Error}", id, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("User updated successfully: {UserId}", id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Deleta um usuário (soft delete)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        var deletedBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _userService.DeleteAsync(id, deletedBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete user: {UserId}. Reason: {Error}", id, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("User deleted successfully: {UserId}", id);
        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Restaura um usuário deletado
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Usuário restaurado</returns>
    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> RestoreUser(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring user: {UserId}", id);

        var restoredBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _userService.RestoreAsync(id, restoredBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to restore user: {UserId}. Reason: {Error}", id, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("User restored successfully: {UserId}", id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém lista de usuários deletados
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de usuários deletados</returns>
    [HttpGet("deleted")]
    public async Task<IActionResult> GetDeletedUsers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting deleted users");

        var result = await _userService.GetDeletedAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get deleted users: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Atribuir roles a um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="request">Lista de IDs dos roles</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação</returns>
    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesToUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning roles to user: {UserId}", id);

        try
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken);

            if (user == null || user.IsDeleted)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            // Remover roles antigos
            var existingUserRoles = await _context.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync(cancellationToken);
            _context.UserRoles.RemoveRange(existingUserRoles);

            // Adicionar novos roles
            var assignedBy = User.FindFirst("sub")?.Value ?? "system";
            foreach (var roleId in request.RoleIds)
            {
                var role = await _context.Roles.FindAsync(new object[] { roleId }, cancellationToken);
                if (role != null && !role.IsDeleted)
                {
                    var userRole = new UserRole(id, roleId, assignedBy);
                    _context.UserRoles.Add(userRole);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Roles assigned successfully to user: {UserId}", id);
            return Ok(new { message = "Roles assigned successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Obter roles de um usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de roles do usuário</returns>
    [HttpGet("{id}/roles")]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting roles for user: {UserId}", id);

        try
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            var roles = user.UserRoles
                .Select(ur => new
                {
                    ur.Role.Id,
                    ur.Role.Name,
                    ur.Role.Description,
                    ur.AssignedAt
                })
                .ToList();

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

// DTO para atribuir roles a usuários
public record AssignRolesToUserRequest(List<Guid> RoleIds);
