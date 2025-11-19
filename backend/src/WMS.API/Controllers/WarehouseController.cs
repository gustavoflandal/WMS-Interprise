using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Requests;
using WMS.Application.Services;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints para gerenciamento de armazéns
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly ILogger<WarehouseController> _logger;

    public WarehouseController(
        IWarehouseService warehouseService,
        ILogger<WarehouseController> logger)
    {
        _warehouseService = warehouseService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os armazéns do tenant atual
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de armazéns</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllWarehouses(CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Getting all warehouses for tenant: {TenantId}", tenantId);

        var result = await _warehouseService.GetAllByTenantAsync(tenantId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get warehouses for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém um armazém específico por ID
    /// </summary>
    /// <param name="id">ID do armazém</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do armazém</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWarehouse(Guid id, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Getting warehouse {WarehouseId} for tenant: {TenantId}", id, tenantId);

        var result = await _warehouseService.GetByIdAsync(tenantId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get warehouse {WarehouseId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        if (result.Value == null)
        {
            _logger.LogInformation("Warehouse {WarehouseId} not found for tenant: {TenantId}", id, tenantId);
            return NotFound(new { message = "Armazém não encontrado" });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo armazém
    /// </summary>
    /// <param name="request">Dados do armazém</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Armazém criado</returns>
    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }

        var userIdClaim = User.FindFirst("sub")?.Value;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        
        _logger.LogInformation("Creating warehouse for tenant: {TenantId}", tenantId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for warehouse creation");
            return BadRequest(ModelState);
        }

        var result = await _warehouseService.CreateAsync(tenantId, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create warehouse for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Warehouse created successfully for tenant: {TenantId}. WarehouseId: {WarehouseId}", tenantId, result.Value?.Id);
        return CreatedAtAction(nameof(GetWarehouse), new { id = result.Value?.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um armazém existente
    /// </summary>
    /// <param name="id">ID do armazém</param>
    /// <param name="request">Dados atualizados do armazém</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Armazém atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWarehouse(Guid id, [FromBody] UpdateWarehouseRequest request, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }

        var userIdClaim = User.FindFirst("sub")?.Value;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        
        _logger.LogInformation("Updating warehouse {WarehouseId} for tenant: {TenantId}. Request: {@Request}", id, tenantId, request);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for warehouse update. ModelState: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var result = await _warehouseService.UpdateAsync(tenantId, id, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update warehouse {WarehouseId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Warehouse {WarehouseId} updated successfully for tenant: {TenantId}", id, tenantId);
        return Ok(result.Value);
    }

    /// <summary>
    /// Exclui (soft delete) um armazém
    /// </summary>
    /// <param name="id">ID do armazém</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWarehouse(Guid id, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }

        var userIdClaim = User.FindFirst("sub")?.Value;
        Guid? userId = null;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var parsedUserId))
        {
            userId = parsedUserId;
        }
        
        _logger.LogInformation("Deleting warehouse {WarehouseId} for tenant: {TenantId}", id, tenantId);

        var result = await _warehouseService.DeleteAsync(tenantId, id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete warehouse {WarehouseId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Warehouse {WarehouseId} deleted successfully for tenant: {TenantId}", id, tenantId);
        return NoContent();
    }
}
