using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

/// <summary>
/// Implementação do serviço de negócio para ASN (Advance Shipping Notice)
/// </summary>
public class ASNService : IASNService
{
    private readonly IUnitOfWork _unitOfWork;

    public ASNService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ASNResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var asn = await _unitOfWork.ASNs.GetByIdAsync(id, cancellationToken);
        if (asn == null)
            return Result<ASNResponse>.Failure("ASN não encontrada");

        return Result<ASNResponse>.Success(MapToResponse(asn));
    }

    public async Task<Result<ASNResponse>> GetByAsnNumberAsync(string asnNumber, int warehouseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(asnNumber))
            return Result<ASNResponse>.Failure("Número da ASN é obrigatório");

        var asn = await _unitOfWork.ASNs.GetByAsnNumberAsync(asnNumber, warehouseId);
        if (asn == null)
            return Result<ASNResponse>.Failure("ASN não encontrada");

        return Result<ASNResponse>.Success(MapToResponse(asn));
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetAllAsync(cancellationToken);
        var responses = asns.Where(a => a.WarehouseId == warehouseId).Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetPendingAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetPendingAsync(warehouseId);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetOverdueAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetOverdueAsync(warehouseId);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetByStatusAsync(int warehouseId, int status, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        if (status < 0 || status > 7)
            return Result<IEnumerable<ASNResponse>>.Failure("Status inválido");

        var asns = await _unitOfWork.ASNs.GetByStatusAsync(warehouseId, status);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetScheduledForDateAsync(int warehouseId, DateTime date, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetScheduledForDateAsync(warehouseId, date);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        if (from > to)
            return Result<IEnumerable<ASNResponse>>.Failure("Data inicial não pode ser maior que a data final");

        var asns = await _unitOfWork.ASNs.GetByDateRangeAsync(warehouseId, from, to);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetBySupplierAsync(int warehouseId, int supplierId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        if (supplierId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do fornecedor inválido");

        var asns = await _unitOfWork.ASNs.GetBySupplierAsync(warehouseId, supplierId);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetByPriorityAsync(int warehouseId, int priority, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        if (priority < 0 || priority > 3)
            return Result<IEnumerable<ASNResponse>>.Failure("Prioridade inválida");

        var asns = await _unitOfWork.ASNs.GetByPriorityAsync(warehouseId, priority);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetCriticalAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetCriticalAsync(warehouseId);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<ASNResponse>>> GetWithDiscrepanciesAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<ASNResponse>>.Failure("ID do armazém inválido");

        var asns = await _unitOfWork.ASNs.GetWithDiscrepanciesAsync(warehouseId);
        var responses = asns.Select(MapToResponse);
        return Result<IEnumerable<ASNResponse>>.Success(responses);
    }

    public async Task<Result<ASNResponse>> CreateAsync(CreateASNRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        if (request.WarehouseId <= 0)
            return Result<ASNResponse>.Failure("ID do armazém inválido");

        if (string.IsNullOrWhiteSpace(request.AsnNumber))
            return Result<ASNResponse>.Failure("Número da ASN é obrigatório");

        var existingAsn = await _unitOfWork.ASNs.GetByAsnNumberAsync(request.AsnNumber, request.WarehouseId);
        if (existingAsn != null)
            return Result<ASNResponse>.Failure("Já existe uma ASN com este número");

        var asn = new ASN
        {
            WarehouseId = request.WarehouseId,
            AsnNumber = request.AsnNumber,
            SupplierId = request.SupplierId,
            InvoiceNumber = request.InvoiceNumber,
            ScheduledArrivalDate = request.ScheduledArrivalDate,
            ExpectedItemCount = request.ExpectedItemCount,
            ExpectedTotalWeight = request.ExpectedTotalWeight,
            ExpectedTotalVolume = request.ExpectedTotalVolume,
            Priority = (ASNPriority)request.Priority,
            Status = ASNStatus.Scheduled,
            ExternalReference = request.ExternalReference,
            OriginWarehouseId = request.OriginWarehouseId
        };

        await _unitOfWork.ASNs.AddAsync(asn, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ASNResponse>.Success(MapToResponse(asn));
    }

    public async Task<Result> UpdateStatusAsync(Guid asnId, int newStatus, string updatedBy, CancellationToken cancellationToken = default)
    {
        if (newStatus < 0 || newStatus > 7)
            return Result.Failure("Status inválido");

        var asn = await _unitOfWork.ASNs.GetByIdAsync(asnId, cancellationToken);
        if (asn == null)
            return Result.Failure("ASN não encontrada");

        asn.Status = (ASNStatus)newStatus;
        await _unitOfWork.ASNs.UpdateAsync(asn, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> MarkAsInspectedAsync(Guid asnId, int inspectionResult, string inspectedBy, CancellationToken cancellationToken = default)
    {
        if (inspectionResult < 0 || inspectionResult > 3)
            return Result.Failure("Resultado de inspeção inválido");

        var asn = await _unitOfWork.ASNs.GetByIdAsync(asnId, cancellationToken);
        if (asn == null)
            return Result.Failure("ASN não encontrada");

        asn.IsInspected = true;
        asn.InspectionResult = (InspectionResult)inspectionResult;

        await _unitOfWork.ASNs.UpdateAsync(asn, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var asn = await _unitOfWork.ASNs.GetByIdAsync(id, cancellationToken);
        if (asn == null)
            return Result.Failure("ASN não encontrada");

        await _unitOfWork.ASNs.DeleteAsync(asn, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<int>> CountPendingAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.ASNs.CountPendingAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> CountOverdueAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.ASNs.CountOverdueAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<ASNResponse>> GetWithItemsAsync(Guid asnId, CancellationToken cancellationToken = default)
    {
        var asn = await _unitOfWork.ASNs.GetWithItemsAsync(asnId);
        if (asn == null)
            return Result<ASNResponse>.Failure("ASN não encontrada");

        return Result<ASNResponse>.Success(MapToResponse(asn));
    }

    private static ASNResponse MapToResponse(ASN asn)
    {
        return new ASNResponse
        {
            Id = asn.Id,
            AsnNumber = asn.AsnNumber,
            WarehouseId = asn.WarehouseId,
            SupplierId = asn.SupplierId,
            InvoiceNumber = asn.InvoiceNumber,
            ScheduledArrivalDate = asn.ScheduledArrivalDate,
            ActualArrivalDate = asn.ActualArrivalDate,
            Status = (int)asn.Status,
            ExpectedItemCount = asn.ExpectedItemCount,
            ReceivedItemCount = asn.ReceivedItemCount,
            ExpectedTotalWeight = asn.ExpectedTotalWeight,
            ExpectedTotalVolume = asn.ExpectedTotalVolume,
            Priority = (int)asn.Priority,
            IsInspected = asn.IsInspected,
            InspectionResult = (int?)asn.InspectionResult,
            ExternalReference = asn.ExternalReference,
            OriginWarehouseId = asn.OriginWarehouseId,
            CreatedAt = asn.CreatedAt,
            UpdatedAt = asn.UpdatedAt ?? DateTime.UtcNow
        };
    }
}
