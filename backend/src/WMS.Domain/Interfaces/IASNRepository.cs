using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Interface para o repositório de ASN (Advance Shipping Notice)
    /// Define operações de acesso a dados para avisos de remessa
    /// </summary>
    public interface IASNRepository : IRepository<ASN>
    {
        /// <summary>
        /// Obtém uma ASN pelo seu número
        /// </summary>
        Task<ASN?> GetByAsnNumberAsync(string asnNumber, int warehouseId);

        /// <summary>
        /// Obtém ASNs pendentes (não recebidas) de um armazém
        /// </summary>
        Task<IEnumerable<ASN>> GetPendingAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs por status
        /// </summary>
        Task<IEnumerable<ASN>> GetByStatusAsync(int warehouseId, int status);

        /// <summary>
        /// Obtém ASNs agendadas para uma data
        /// </summary>
        Task<IEnumerable<ASN>> GetScheduledForDateAsync(int warehouseId, DateTime date);

        /// <summary>
        /// Obtém ASNs vencidas (data prevista passou, sem recebimento)
        /// </summary>
        Task<IEnumerable<ASN>> GetOverdueAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs por período de datas
        /// </summary>
        Task<IEnumerable<ASN>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to);

        /// <summary>
        /// Obtém ASNs de um fornecedor específico
        /// </summary>
        Task<IEnumerable<ASN>> GetBySupplierAsync(int warehouseId, int supplierId);

        /// <summary>
        /// Obtém ASNs por prioridade
        /// </summary>
        Task<IEnumerable<ASN>> GetByPriorityAsync(int warehouseId, int priority);

        /// <summary>
        /// Obtém ASNs críticas/de alta prioridade
        /// </summary>
        Task<IEnumerable<ASN>> GetCriticalAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs que foram inspecionadas
        /// </summary>
        Task<IEnumerable<ASN>> GetInspectedAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs pendentes de inspeção
        /// </summary>
        Task<IEnumerable<ASN>> GetPendingInspectionAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs rejeitadas
        /// </summary>
        Task<IEnumerable<ASN>> GetRejectedAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs por referência externa
        /// </summary>
        Task<ASN?> GetByExternalReferenceAsync(int warehouseId, string externalReference);

        /// <summary>
        /// Conta ASNs pendentes
        /// </summary>
        Task<int> CountPendingAsync(int warehouseId);

        /// <summary>
        /// Conta ASNs vencidas
        /// </summary>
        Task<int> CountOverdueAsync(int warehouseId);

        /// <summary>
        /// Obtém ASNs com discrepâncias (items não conformes)
        /// </summary>
        Task<IEnumerable<ASN>> GetWithDiscrepanciesAsync(int warehouseId);

        /// <summary>
        /// Atualiza o status de uma ASN
        /// </summary>
        Task<bool> UpdateStatusAsync(int asnId, int newStatus);

        /// <summary>
        /// Obtém incluindo seus items
        /// </summary>
        Task<ASN?> GetWithItemsAsync(int asnId);

        /// <summary>
        /// Obtém para transferência entre armazéns
        /// </summary>
        Task<IEnumerable<ASN>> GetInternalTransfersAsync(int warehouseId);
    }
}
