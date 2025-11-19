using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Application.Services;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints para gerenciamento de Produtos (SKUs)
/// Módulo: RF-001 (Recebimento e Armazenagem)
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os produtos do tenant
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de produtos</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products");

        var tenantId = GetTenantId();
        var result = await _productService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get products. Error: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do produto</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product by id: {Id}", id);

        var result = await _productService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get product {Id}. Error: {Error}", id, result.Error);
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtém um produto por SKU
    /// </summary>
    /// <param name="sku">SKU do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do produto</returns>
    [HttpGet("sku/{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySku(string sku, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product by SKU: {Sku}", sku);

        var tenantId = GetTenantId();
        var result = await _productService.GetBySkuAsync(sku, tenantId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to get product with SKU {Sku}. Error: {Error}", sku, result.Error);
            return NotFound(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="request">Dados do novo produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do produto criado</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new product with SKU: {Sku}", request.Sku);

        var tenantId = GetTenantId();
        var userId = GetUserId();

        var result = await _productService.CreateAsync(request, tenantId, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to create product. Error: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value?.Id }, result.Value);
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="request">Dados a atualizar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do produto atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product: {Id}", id);

        var userId = GetUserId();
        var result = await _productService.UpdateAsync(id, request, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update product {Id}. Error: {Error}", id, result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Deleta um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Status de sucesso</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting product: {Id}", id);

        var userId = GetUserId();
        var result = await _productService.DeleteAsync(id, userId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to delete product {Id}. Error: {Error}", id, result.Error);
            return NotFound(new { message = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Verifica se um SKU já existe
    /// </summary>
    /// <param name="sku">SKU a verificar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>true se existe, false caso contrário</returns>
    [HttpGet("check-sku/{sku}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> CheckSkuExists(string sku, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking if SKU exists: {Sku}", sku);

        var tenantId = GetTenantId();
        var result = await _productService.SkuExistsAsync(sku, tenantId, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to check SKU. Error: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Extrai o tenant_id do JWT token
    /// </summary>
    private int GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        return string.IsNullOrEmpty(tenantIdClaim) ? 0 : int.Parse(tenantIdClaim);
    }

    /// <summary>
    /// Extrai o user_id do JWT token
    /// </summary>
    private string GetUserId()
    {
        return User.FindFirst("sub")?.Value ?? User.Identity?.Name ?? "system";
    }
}
