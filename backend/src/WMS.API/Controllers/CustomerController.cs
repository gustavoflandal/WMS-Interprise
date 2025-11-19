using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Requests;
using WMS.Application.Services;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints para gerenciamento de clientes
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(
        ICustomerService customerService,
        ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os clientes do tenant atual
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de clientes</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllCustomers(CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }

        _logger.LogInformation("Getting all customers for tenant: {TenantId}", tenantId);

        var result = await _customerService.GetAllByTenantAsync(tenantId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get customers for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém um cliente específico por ID
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do cliente</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }

        _logger.LogInformation("Getting customer {CustomerId} for tenant: {TenantId}", id, tenantId);

        var result = await _customerService.GetByIdAsync(tenantId, id, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get customer {CustomerId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        if (result.Value == null)
        {
            _logger.LogInformation("Customer {CustomerId} not found for tenant: {TenantId}", id, tenantId);
            return NotFound(new { message = "Cliente não encontrado" });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo cliente
    /// </summary>
    /// <param name="request">Dados do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Cliente criado</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
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

        _logger.LogInformation("Creating customer for tenant: {TenantId}", tenantId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for customer creation");
            return BadRequest(ModelState);
        }

        var result = await _customerService.CreateAsync(tenantId, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create customer for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Customer created successfully for tenant: {TenantId}. CustomerId: {CustomerId}", tenantId, result.Value?.Id);
        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value?.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um cliente existente
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="request">Dados atualizados do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Cliente atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
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

        _logger.LogInformation("Updating customer {CustomerId} for tenant: {TenantId}. Request: {@Request}", id, tenantId, request);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for customer update. ModelState: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var result = await _customerService.UpdateAsync(tenantId, id, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update customer {CustomerId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Customer {CustomerId} updated successfully for tenant: {TenantId}", id, tenantId);
        return Ok(result.Value);
    }

    /// <summary>
    /// Exclui (soft delete) um cliente
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
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

        _logger.LogInformation("Deleting customer {CustomerId} for tenant: {TenantId}", id, tenantId);

        var result = await _customerService.DeleteAsync(tenantId, id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete customer {CustomerId} for tenant: {TenantId}. Error: {Error}", id, tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Customer {CustomerId} deleted successfully for tenant: {TenantId}", id, tenantId);
        return NoContent();
    }
}
