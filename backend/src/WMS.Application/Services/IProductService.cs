using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

/// <summary>
/// Serviço de negócio para Produtos
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um produto por SKU
    /// </summary>
    Task<Result<ProductResponse>> GetBySkuAsync(string sku, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os produtos ativos
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetActiveAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos por categoria
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetByCategoryAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos por classificação ABC
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetByABCClassificationAsync(int classification, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos que requerem rastreamento de lote
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetRequiringLotTrackingAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos que requerem número de série
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetRequiringSerialNumberAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos perecíveis
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetPerishableAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém produtos que requerem controle de temperatura
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> GetRequiringTemperatureControlAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca produtos por termo de pesquisa
    /// </summary>
    Task<Result<IEnumerable<ProductResponse>>> SearchAsync(string searchTerm, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, int tenantId, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um produto
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um SKU já existe
    /// </summary>
    Task<Result<bool>> SkuExistsAsync(string sku, int tenantId, CancellationToken cancellationToken = default);
}
