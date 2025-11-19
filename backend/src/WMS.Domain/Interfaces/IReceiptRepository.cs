using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces
{
    /// <summary>
    /// Interface para o repositório de Documentação de Recebimento
    /// Define operações de acesso a dados para recebimentos
    /// </summary>
    public interface IReceiptRepository : IRepository<ReceiptDocumentation>
    {
        /// <summary>
        /// Obtém um recebimento pelo seu número
        /// </summary>
        Task<ReceiptDocumentation?> GetByReceiptNumberAsync(string receiptNumber, int warehouseId);

        /// <summary>
        /// Obtém recebimentos de um armazém em uma data específica
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetByDateAsync(int warehouseId, DateTime date);

        /// <summary>
        /// Obtém recebimentos por período
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to);

        /// <summary>
        /// Obtém recebimentos por status
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetByStatusAsync(int warehouseId, int status);

        /// <summary>
        /// Obtém recebimentos em andamento
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetInProgressAsync(int warehouseId);

        /// <summary>
        /// Obtém recebimentos pendentes de confirmação
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetPendingConfirmationAsync(int warehouseId);

        /// <summary>
        /// Obtém recebimentos com discrepâncias
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetWithDiscrepanciesAsync(int warehouseId);

        /// <summary>
        /// Obtém recebimentos de um fornecedor específico
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetBySupplierAsync(int warehouseId, int supplierId);

        /// <summary>
        /// Obtém recebimentos por tipo
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetByTypeAsync(int warehouseId, int receiptType);

        /// <summary>
        /// Obtém recebimentos por ASN
        /// </summary>
        Task<ReceiptDocumentation?> GetByAsnAsync(int asnId);

        /// <summary>
        /// Obtém recebimentos por número de nota fiscal
        /// </summary>
        Task<ReceiptDocumentation?> GetByInvoiceAsync(int warehouseId, string invoiceNumber);

        /// <summary>
        /// Obtém recebimentos por referência externa
        /// </summary>
        Task<ReceiptDocumentation?> GetByExternalReferenceAsync(int warehouseId, string externalReference);

        /// <summary>
        /// Obtém recebimentos realizados por um operador
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetByOperatorAsync(int warehouseId, int operatorId, DateTime? date = null);

        /// <summary>
        /// Obtém recebimentos confirmados por um supervisor
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetConfirmedBySupervisorAsync(int warehouseId, int supervisorId, DateTime? date = null);

        /// <summary>
        /// Conta recebimentos em progresso
        /// </summary>
        Task<int> CountInProgressAsync(int warehouseId);

        /// <summary>
        /// Conta recebimentos pendentes de confirmação
        /// </summary>
        Task<int> CountPendingConfirmationAsync(int warehouseId);

        /// <summary>
        /// Conta recebimentos com discrepâncias
        /// </summary>
        Task<int> CountWithDiscrepanciesAsync(int warehouseId);

        /// <summary>
        /// Obtém tempo médio de recebimento
        /// </summary>
        Task<int> GetAverageReceiptTimeAsync(int warehouseId);

        /// <summary>
        /// Obtém incluindo seus items
        /// </summary>
        Task<ReceiptDocumentation?> GetWithItemsAsync(Guid receiptId);

        /// <summary>
        /// Obtém recebimentos não fechados (rascunho ou em progresso)
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetUnfinishedAsync(int warehouseId);

        /// <summary>
        /// Atualiza o status de um recebimento
        /// </summary>
        Task<bool> UpdateStatusAsync(Guid receiptId, int newStatus);

        /// <summary>
        /// Obtém recebimentos em quarentena
        /// </summary>
        Task<IEnumerable<ReceiptDocumentation>> GetOnHoldAsync(int warehouseId);

        /// <summary>
        /// Obtém próximo número de recebimento disponível
        /// </summary>
        Task<string> GetNextReceiptNumberAsync(int warehouseId);
    }
}
