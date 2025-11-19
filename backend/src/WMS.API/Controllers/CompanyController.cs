using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Requests;
using WMS.Application.Services;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints para gerenciamento de empresa
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(
        ICompanyService companyService,
        ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém os dados da empresa do tenant atual
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados da empresa ou null se não existir</returns>
    [HttpGet]
    public async Task<IActionResult> GetCompany(CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Getting company for tenant: {TenantId}", tenantId);

        var result = await _companyService.GetByTenantAsync(tenantId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get company for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        if (result.Value == null)
        {
            _logger.LogInformation("No company found for tenant: {TenantId}", tenantId);
            return NotFound(new { message = "Empresa não encontrada" });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria uma nova empresa para o tenant atual
    /// </summary>
    /// <param name="request">Dados da empresa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Empresa criada</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Creating company for tenant: {TenantId}", tenantId);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for company creation");
            return BadRequest(ModelState);
        }

        var createdBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _companyService.CreateAsync(tenantId, request, createdBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create company for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Company created successfully for tenant: {TenantId}", tenantId);
        return CreatedAtAction(nameof(GetCompany), new { }, result.Value);
    }

    /// <summary>
    /// Atualiza os dados da empresa do tenant atual
    /// </summary>
    /// <param name="request">Dados atualizados da empresa</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Empresa atualizada</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyRequest request, CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Updating company for tenant: {TenantId}. Request: {@Request}", tenantId, request);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for company update. ModelState: {@ModelState}", ModelState);
            return BadRequest(ModelState);
        }

        var updatedBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _companyService.UpdateAsync(tenantId, request, updatedBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update company for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Company updated successfully for tenant: {TenantId}", tenantId);
        return Ok(result.Value);
    }

    /// <summary>
    /// Exclui (soft delete) a empresa do tenant atual
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteCompany(CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantIdClaim) || !Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _logger.LogWarning("Invalid or missing tenant_id in token");
            return BadRequest(new { message = "Tenant não identificado" });
        }
        
        _logger.LogInformation("Deleting company for tenant: {TenantId}", tenantId);

        var deletedBy = User.FindFirst("sub")?.Value ?? "system";
        var result = await _companyService.DeleteAsync(tenantId, deletedBy, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete company for tenant: {TenantId}. Error: {Error}", tenantId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Company deleted successfully for tenant: {TenantId}", tenantId);
        return NoContent();
    }
}
