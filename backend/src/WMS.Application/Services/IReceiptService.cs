using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

/// <summary>
/// Serviço de negócio para Recebimentos
/// </summary>
public interface IReceiptService
{
    /// <summary>
    /// Obtém um recebimento por ID
    /// </summary>
    Task<Result<ReceiptResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um recebimento por número
    /// </summary>
    Task<Result<ReceiptResponse>> GetByReceiptNumberAsync(string receiptNumber, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os recebimentos do armazém
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos por data
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetByDateAsync(int warehouseId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos em um intervalo de datas
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos por status
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetByStatusAsync(int warehouseId, int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos em andamento
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetInProgressAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos não confirmados
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetPendingConfirmationAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos com discrepâncias
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos por tipo
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetByTypeAsync(int warehouseId, int receiptType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos por fornecedor
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetBySupplierAsync(int warehouseId, int supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos por operador
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetByOperatorAsync(int warehouseId, int operatorId, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos confirmados por supervisor
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetConfirmedBySupervisorAsync(int warehouseId, int supervisorId, DateTime? date = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimentos não terminados
    /// </summary>
    Task<Result<IEnumerable<ReceiptResponse>>> GetUnfinishedAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém próximo número de recebimento
    /// </summary>
    Task<Result<string>> GetNextReceiptNumberAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo recebimento
    /// </summary>
    Task<Result<ReceiptResponse>> CreateAsync(CreateReceiptRequest request, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o status de um recebimento
    /// </summary>
    Task<Result> UpdateStatusAsync(Guid receiptId, int newStatus, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma um recebimento
    /// </summary>
    Task<Result> ConfirmAsync(Guid receiptId, string confirmedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um recebimento
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos recebimentos estão em andamento
    /// </summary>
    Task<Result<int>> CountInProgressAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos recebimentos estão aguardando confirmação
    /// </summary>
    Task<Result<int>> CountPendingConfirmationAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos recebimentos têm discrepâncias
    /// </summary>
    Task<Result<int>> CountWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém tempo médio de recebimento em minutos
    /// </summary>
    Task<Result<int>> GetAverageReceiptTimeAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém recebimento com seus itens
    /// </summary>
    Task<Result<ReceiptResponse>> GetWithItemsAsync(Guid receiptId, CancellationToken cancellationToken = default);
}
