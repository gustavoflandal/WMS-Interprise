using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

/// <summary>
/// Serviço de negócio para ASN (Advance Shipping Notice)
/// </summary>
public interface IASNService
{
    /// <summary>
    /// Obtém um ASN por ID
    /// </summary>
    Task<Result<ASNResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um ASN por número
    /// </summary>
    Task<Result<ASNResponse>> GetByAsnNumberAsync(string asnNumber, int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém todos os ASNs do armazém
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs pendentes de recebimento
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetPendingAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs atrasados
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetOverdueAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs por status
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetByStatusAsync(int warehouseId, int status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs agendados para uma data específica
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetScheduledForDateAsync(int warehouseId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs em um intervalo de datas
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs por fornecedor
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetBySupplierAsync(int warehouseId, int supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs por prioridade
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetByPriorityAsync(int warehouseId, int priority, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs críticos (prioridade alta ou crítica)
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetCriticalAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASNs com discrepâncias
    /// </summary>
    Task<Result<IEnumerable<ASNResponse>>> GetWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo ASN
    /// </summary>
    Task<Result<ASNResponse>> CreateAsync(CreateASNRequest request, string createdBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza o status de um ASN
    /// </summary>
    Task<Result> UpdateStatusAsync(Guid asnId, int newStatus, string updatedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marca um ASN como inspecionado
    /// </summary>
    Task<Result> MarkAsInspectedAsync(Guid asnId, int inspectionResult, string inspectedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um ASN
    /// </summary>
    Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos ASNs estão pendentes
    /// </summary>
    Task<Result<int>> CountPendingAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta quantos ASNs estão atrasados
    /// </summary>
    Task<Result<int>> CountOverdueAsync(int warehouseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém ASN com seus itens
    /// </summary>
    Task<Result<ASNResponse>> GetWithItemsAsync(Guid asnId, CancellationToken cancellationToken = default);
}
