using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Interface para o repositório de Localizações de Armazenamento
    /// Define operações de acesso a dados para localizações
    /// </summary>
    public interface IStorageLocationRepository : IRepository<StorageLocation>
    {
        /// <summary>
        /// Obtém uma localização pelo seu código
        /// </summary>
        Task<StorageLocation?> GetByCodeAsync(string code, int warehouseId);

        /// <summary>
        /// Obtém todas as localizações disponíveis em um armazém
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetAvailableAsync(int warehouseId);

        /// <summary>
        /// Obtém localizações por zona
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetByZoneAsync(int warehouseId, int zone);

        /// <summary>
        /// Obtém todas as localizações ocupadas
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetOccupiedAsync(int warehouseId);

        /// <summary>
        /// Obtém localizações parcialmente ocupadas
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetPartiallyOccupiedAsync(int warehouseId);

        /// <summary>
        /// Obtém localizações indisponíveis/bloqueadas
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetUnavailableAsync(int warehouseId);

        /// <summary>
        /// Obtém localizações que requerem controle de temperatura
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetTemperatureControlledAsync(int warehouseId);

        /// <summary>
        /// Obtém localizações por corredor
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetByCorridorAsync(int warehouseId, string corridorNumber);

        /// <summary>
        /// Obtém localizações mais próximas do ponto de consolidação
        /// Ordenadas por distância
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetNearestToConsolidationAsync(int warehouseId, int limit);

        /// <summary>
        /// Verifica se uma localização está disponível e pode armazenar um produto
        /// </summary>
        Task<bool> CanAccommodateProductAsync(Guid locationId, int productId, int quantity);

        /// <summary>
        /// Obtém localizações que armazenam um produto específico
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetByProductAsync(int productId, int warehouseId);

        /// <summary>
        /// Obtém localizações com capacidade disponível para um produto
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetWithCapacityAsync(int warehouseId, int requiredCapacity);

        /// <summary>
        /// Obtém a próxima localização disponível em sequência
        /// </summary>
        Task<StorageLocation?> GetNextAvailableAsync(int warehouseId, int zone);

        /// <summary>
        /// Conta o total de localizações disponíveis
        /// </summary>
        Task<int> CountAvailableAsync(int warehouseId);

        /// <summary>
        /// Conta o total de localizações ocupadas
        /// </summary>
        Task<int> CountOccupiedAsync(int warehouseId);

        /// <summary>
        /// Obtém a utilização percentual do armazém
        /// </summary>
        Task<decimal> GetUtilizationPercentageAsync(int warehouseId);

        /// <summary>
        /// Atualiza o status de uma localização
        /// </summary>
        Task<bool> UpdateStatusAsync(Guid locationId, int newStatus);

        /// <summary>
        /// Bloqueia uma localização
        /// </summary>
        Task<bool> BlockLocationAsync(Guid locationId, string reason);

        /// <summary>
        /// Desbloqueia uma localização
        /// </summary>
        Task<bool> UnblockLocationAsync(Guid locationId);

        /// <summary>
        /// Registra um acesso à localização
        /// </summary>
        Task<bool> RecordAccessAsync(Guid locationId);

        /// <summary>
        /// Obtém localizações órfãs (ocupadas mas sem produto válido)
        /// </summary>
        Task<IEnumerable<StorageLocation>> GetOrphanedAsync(int warehouseId);
    }
}
