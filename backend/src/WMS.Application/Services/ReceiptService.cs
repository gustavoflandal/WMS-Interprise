using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

/// <summary>
/// Implementação do serviço de negócio para Recebimentos
/// </summary>
public class ReceiptService : IReceiptService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReceiptService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ReceiptResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var receipt = await _unitOfWork.Receipts.GetByIdAsync(id);
        if (receipt == null)
            return Result<ReceiptResponse>.Failure("Recebimento não encontrado");

        return Result<ReceiptResponse>.Success(MapToResponse(receipt));
    }

    public async Task<Result<ReceiptResponse>> GetByReceiptNumberAsync(string receiptNumber, int warehouseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(receiptNumber))
            return Result<ReceiptResponse>.Failure("Número do recebimento é obrigatório");

        var receipt = await _unitOfWork.Receipts.GetByReceiptNumberAsync(receiptNumber, warehouseId);
        if (receipt == null)
            return Result<ReceiptResponse>.Failure("Recebimento não encontrado");

        return Result<ReceiptResponse>.Success(MapToResponse(receipt));
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetAllAsync();
        var responses = receipts.Where(r => r.WarehouseId == warehouseId).Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetByDateAsync(int warehouseId, DateTime date, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetByDateAsync(warehouseId, date);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (from > to)
            return Result<IEnumerable<ReceiptResponse>>.Failure("Data inicial não pode ser maior que a data final");

        var receipts = await _unitOfWork.Receipts.GetByDateRangeAsync(warehouseId, from, to);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetByStatusAsync(int warehouseId, int status, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (status < 0 || status > 5)
            return Result<IEnumerable<ReceiptResponse>>.Failure("Status inválido");

        var receipts = await _unitOfWork.Receipts.GetByStatusAsync(warehouseId, status);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetInProgressAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetInProgressAsync(warehouseId);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetPendingConfirmationAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetPendingConfirmationAsync(warehouseId);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetWithDiscrepanciesAsync(warehouseId);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetByTypeAsync(int warehouseId, int receiptType, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (receiptType < 0 || receiptType > 5)
            return Result<IEnumerable<ReceiptResponse>>.Failure("Tipo de recebimento inválido");

        var receipts = await _unitOfWork.Receipts.GetByTypeAsync(warehouseId, receiptType);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetBySupplierAsync(int warehouseId, int supplierId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (supplierId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do fornecedor inválido");

        var receipts = await _unitOfWork.Receipts.GetBySupplierAsync(warehouseId, supplierId);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetByOperatorAsync(int warehouseId, int operatorId, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (operatorId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do operador inválido");

        var receipts = await _unitOfWork.Receipts.GetByOperatorAsync(warehouseId, operatorId, date);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetConfirmedBySupervisorAsync(int warehouseId, int supervisorId, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        if (supervisorId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do supervisor inválido");

        var receipts = await _unitOfWork.Receipts.GetConfirmedBySupervisorAsync(warehouseId, supervisorId, date);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ReceiptResponse>>> GetUnfinishedAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ReceiptResponse>>.Failure("ID do armazém inválido");

        var receipts = await _unitOfWork.Receipts.GetUnfinishedAsync(warehouseId);
        var responses = receipts.Select(MapToResponse);
        return Result<IEnumerable<ReceiptResponse>>.Success(responses);
    }

    public async Task<Result<string>> GetNextReceiptNumberAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<string>.Failure("ID do armazém inválido");

        var nextNumber = await _unitOfWork.Receipts.GetNextReceiptNumberAsync(warehouseId);
        return Result<string>.Success(nextNumber);
    }

    public async Task<Result<ReceiptResponse>> CreateAsync(CreateReceiptRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        if (request.WarehouseId <= 0)
            return Result<ReceiptResponse>.Failure("ID do armazém inválido");

        if (request.OperatorId <= 0)
            return Result<ReceiptResponse>.Failure("ID do operador inválido");

        // Gerar próximo número de recebimento
        var nextNumber = await _unitOfWork.Receipts.GetNextReceiptNumberAsync(request.WarehouseId);

        var receipt = new ReceiptDocumentation
        {
            WarehouseId = request.WarehouseId,
            ReceiptNumber = nextNumber,
            ReceiptDate = DateTime.UtcNow,
            AsnId = request.AsnId,
            Status = ReceiptStatus.Draft,
            ReceiptType = (ReceiptType)request.ReceiptType,
            InvoiceNumber = request.InvoiceNumber,
            OperatorId = request.OperatorId,
            SupervisorId = request.SupervisorId,
            ExternalReference = request.ExternalReference,
            TotalItemsReceived = 0,
            TotalWeight = 0,
            HasDiscrepancies = false
        };

        await _unitOfWork.Receipts.AddAsync(receipt);
        await _unitOfWork.SaveChangesAsync();

        return Result<ReceiptResponse>.Success(MapToResponse(receipt));
    }

    public async Task<Result> UpdateStatusAsync(int receiptId, int newStatus, string updatedBy, CancellationToken cancellationToken = default)
    {
        if (receiptId <= 0)
            return Result.Failure("ID do recebimento inválido");

        if (newStatus < 0 || newStatus > 5)
            return Result.Failure("Status inválido");

        var updated = await _unitOfWork.Receipts.UpdateStatusAsync(receiptId, newStatus);
        if (!updated)
            return Result.Failure("Recebimento não encontrado");

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> ConfirmAsync(int receiptId, string confirmedBy, CancellationToken cancellationToken = default)
    {
        if (receiptId <= 0)
            return Result.Failure("ID do recebimento inválido");

        var receipt = await _unitOfWork.Receipts.GetByIdAsync(receiptId);
        if (receipt == null)
            return Result.Failure("Recebimento não encontrado");

        receipt.Status = ReceiptStatus.Confirmed;
        receipt.ConfirmedAt = DateTime.UtcNow;
        receipt.UpdatedBy = confirmedBy;

        await _unitOfWork.Receipts.UpdateAsync(receipt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var receipt = await _unitOfWork.Receipts.GetByIdAsync(id);
        if (receipt == null)
            return Result.Failure("Recebimento não encontrado");

        receipt.DeletedBy = deletedBy;
        await _unitOfWork.Receipts.DeleteAsync(receipt);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<int>> CountInProgressAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.Receipts.CountInProgressAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> CountPendingConfirmationAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.Receipts.CountPendingConfirmationAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> CountWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.Receipts.CountWithDiscrepanciesAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> GetAverageReceiptTimeAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var average = await _unitOfWork.Receipts.GetAverageReceiptTimeAsync(warehouseId);
        return Result<int>.Success(average);
    }

    public async Task<Result<ReceiptResponse>> GetWithItemsAsync(int receiptId, CancellationToken cancellationToken = default)
    {
        if (receiptId <= 0)
            return Result<ReceiptResponse>.Failure("ID do recebimento inválido");

        var receipt = await _unitOfWork.Receipts.GetWithItemsAsync(receiptId);
        if (receipt == null)
            return Result<ReceiptResponse>.Failure("Recebimento não encontrado");

        return Result<ReceiptResponse>.Success(MapToResponse(receipt));
    }

    private static ReceiptResponse MapToResponse(ReceiptDocumentation receipt)
    {
        return new ReceiptResponse
        {
            Id = receipt.Id.GetHashCode(), // Convert Guid to int
            ReceiptNumber = receipt.ReceiptNumber,
            WarehouseId = receipt.WarehouseId,
            AsnId = receipt.AsnId,
            ReceiptDate = receipt.ReceiptDate,
            Status = (int)receipt.Status,
            ReceiptType = (int)receipt.ReceiptType,
            TotalItemsReceived = receipt.TotalItemsReceived,
            TotalWeight = receipt.TotalWeight,
            OperatorId = receipt.OperatorId,
            SupervisorId = receipt.SupervisorId,
            InvoiceNumber = receipt.InvoiceNumber,
            HasDiscrepancies = receipt.HasDiscrepancies,
            TotalReceiptTimeMinutes = receipt.TotalReceiptTimeMinutes,
            ConfirmedAt = receipt.ConfirmedAt,
            ExternalReference = receipt.ExternalReference,
            CreatedAt = receipt.CreatedAt,
            UpdatedAt = receipt.UpdatedAt ?? DateTime.UtcNow
        };
    }
}
