using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

/// <summary>
/// Serviço de negócio para Localizações de Armazenamento
/// </summary>
public interface IStorageLocationService
{
    /// <summary>
    /// Obtém uma localização por ID
    /// </summary>
    Task<Result<StorageLocationResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém uma localização por código
    /// </summary>
    Task<Result<StorageLocationResponse>> GetByCodeAsync(string code, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todas as localizações do armazém
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações disponíveis
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetAvailableAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações ocupadas
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações parcialmente ocupadas
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetPartiallyOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações indisponíveis
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetUnavailableAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações por zona
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetByZoneAsync(int warehouseId, int zone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações com controle de temperatura
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetTemperatureControlledAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações mais próximas do ponto de consolidação
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetNearestToConsolidationAsync(int warehouseId, int limit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações por produto
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetByProductAsync(int productId, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações com capacidade suficiente
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetWithCapacityAsync(int warehouseId, int requiredCapacity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém próxima localização disponível em uma zona
    /// </summary>
    Task<Result<StorageLocationResponse>> GetNextAvailableAsync(int warehouseId, int zone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém localizações órfãs (com quantidade mas sem produto)
    /// </summary>
    Task<Result<IEnumerable<StorageLocationResponse>>> GetOrphanedAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria uma nova localização de armazenamento
    /// </summary>
    Task<Result<StorageLocationResponse>> CreateAsync(CreateStorageLocationRequest request, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o status de uma localização
    /// </summary>
    Task<Result> UpdateStatusAsync(int locationId, int newStatus, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bloqueia uma localização
    /// </summary>
    Task<Result> BlockLocationAsync(int locationId, string reason, string blockedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Desbloqueia uma localização
    /// </summary>
    Task<Result> UnblockLocationAsync(int locationId, string unblockedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra acesso a uma localização
    /// </summary>
    Task<Result> RecordAccessAsync(int locationId, string accessedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se uma localização pode acomodar um produto
    /// </summary>
    Task<Result<bool>> CanAccommodateProductAsync(int locationId, int productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta uma localização
    /// </summary>
    Task<Result> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta localizações disponíveis
    /// </summary>
    Task<Result<int>> CountAvailableAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta localizações ocupadas
    /// </summary>
    Task<Result<int>> CountOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém percentual de utilização do armazém
    /// </summary>
    Task<Result<decimal>> GetUtilizationPercentageAsync(int warehouseId, CancellationToken cancellationToken = default);
}
