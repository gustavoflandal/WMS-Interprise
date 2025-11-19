using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Enums;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

/// <summary>
/// Implementação do serviço de negócio para Localizações de Armazenamento
/// </summary>
public class StorageLocationService : IStorageLocationService
{
    private readonly IUnitOfWork _unitOfWork;

    public StorageLocationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StorageLocationResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await _unitOfWork.StorageLocations.GetByIdAsync(id, cancellationToken);
        if (location == null)
            return Result<StorageLocationResponse>.Failure("Localização não encontrada");

        return Result<StorageLocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<StorageLocationResponse>> GetByCodeAsync(string code, int warehouseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<StorageLocationResponse>.Failure("Código é obrigatório");

        var location = await _unitOfWork.StorageLocations.GetByCodeAsync(code, warehouseId);
        if (location == null)
            return Result<StorageLocationResponse>.Failure("Localização não encontrada");

        return Result<StorageLocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetAllAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetAllAsync(cancellationToken);
        var responses = locations.Where(l => l.WarehouseId == warehouseId).Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetAvailableAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetAvailableAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetOccupiedAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetPartiallyOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetPartiallyOccupiedAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetUnavailableAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetUnavailableAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetByZoneAsync(int warehouseId, int zone, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        if (zone < 0 || zone > 9)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("Zona inválida");

        var locations = await _unitOfWork.StorageLocations.GetByZoneAsync(warehouseId, zone);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetTemperatureControlledAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetTemperatureControlledAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetNearestToConsolidationAsync(int warehouseId, int limit, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        if (limit <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("Limite deve ser maior que 0");

        var locations = await _unitOfWork.StorageLocations.GetNearestToConsolidationAsync(warehouseId, limit);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetByProductAsync(int productId, int warehouseId, CancellationToken cancellationToken = default)
    {
        if (productId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do produto inválido");

        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetByProductAsync(productId, warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetWithCapacityAsync(int warehouseId, int requiredCapacity, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        if (requiredCapacity <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("Capacidade requerida deve ser maior que 0");

        var locations = await _unitOfWork.StorageLocations.GetWithCapacityAsync(warehouseId, requiredCapacity);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<StorageLocationResponse>> GetNextAvailableAsync(int warehouseId, int zone, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<StorageLocationResponse>.Failure("ID do armazém inválido");

        if (zone < 0 || zone > 9)
            return Result<StorageLocationResponse>.Failure("Zona inválida");

        var location = await _unitOfWork.StorageLocations.GetNextAvailableAsync(warehouseId, zone);
        if (location == null)
            return Result<StorageLocationResponse>.Failure("Nenhuma localização disponível encontrada");

        return Result<StorageLocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result<IEnumerable<StorageLocationResponse>>> GetOrphanedAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<IEnumerable<StorageLocationResponse>>.Failure("ID do armazém inválido");

        var locations = await _unitOfWork.StorageLocations.GetOrphanedAsync(warehouseId);
        var responses = locations.Select(MapToResponse);
        return Result<IEnumerable<StorageLocationResponse>>.Success(responses);
    }

    public async Task<Result<StorageLocationResponse>> CreateAsync(CreateStorageLocationRequest request, string createdBy, CancellationToken cancellationToken = default)
    {
        if (request.WarehouseId <= 0)
            return Result<StorageLocationResponse>.Failure("ID do armazém inválido");

        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<StorageLocationResponse>.Failure("Código é obrigatório");

        var existingLocation = await _unitOfWork.StorageLocations.GetByCodeAsync(request.Code, request.WarehouseId);
        if (existingLocation != null)
            return Result<StorageLocationResponse>.Failure("Já existe uma localização com este código");

        var location = new StorageLocation
        {
            WarehouseId = request.WarehouseId,
            Code = request.Code,
            Zone = (StorageZone)request.Zone,
            Status = LocationStatus.Available,
            MaxCapacityKg = request.MaxCapacityKg,
            MaxCapacityUnits = request.MaxCapacityUnits,
            RowPosition = request.RowPosition,
            ColumnPosition = request.ColumnPosition,
            LevelPosition = request.LevelPosition,
            CorridorNumber = request.CorridorNumber,
            DistanceFromConsolidationPoint = request.DistanceFromConsolidationPoint,
            RequiresTemperatureControl = request.RequiresTemperatureControl,
            IdealTemperature = request.MinTemperature,
            IdealHumidity = request.MinHumidity
        };

        await _unitOfWork.StorageLocations.AddAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StorageLocationResponse>.Success(MapToResponse(location));
    }

    public async Task<Result> UpdateStatusAsync(Guid locationId, int newStatus, string updatedBy, CancellationToken cancellationToken = default)
    {
        if (newStatus < 1 || newStatus > 4)
            return Result.Failure("Status inválido");

        var location = await _unitOfWork.StorageLocations.GetByIdAsync(locationId, cancellationToken);
        if (location == null)
            return Result.Failure("Localização não encontrada");

        location.Status = (LocationStatus)newStatus;
        await _unitOfWork.StorageLocations.UpdateAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> BlockLocationAsync(Guid locationId, string reason, string blockedBy, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure("Motivo do bloqueio é obrigatório");

        var location = await _unitOfWork.StorageLocations.GetByIdAsync(locationId, cancellationToken);
        if (location == null)
            return Result.Failure("Localização não encontrada");

        location.IsBlocked = true;
        location.BlockReason = reason;
        location.Status = LocationStatus.Unavailable;

        await _unitOfWork.StorageLocations.UpdateAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> UnblockLocationAsync(Guid locationId, string unblockedBy, CancellationToken cancellationToken = default)
    {
        var location = await _unitOfWork.StorageLocations.GetByIdAsync(locationId, cancellationToken);
        if (location == null)
            return Result.Failure("Localização não encontrada");

        location.IsBlocked = false;
        location.BlockReason = null;

        await _unitOfWork.StorageLocations.UpdateAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RecordAccessAsync(Guid locationId, string accessedBy, CancellationToken cancellationToken = default)
    {
        var location = await _unitOfWork.StorageLocations.GetByIdAsync(locationId, cancellationToken);
        if (location == null)
            return Result.Failure("Localização não encontrada");

        location.AccessCount++;
        location.LastRemovalDate = DateTime.UtcNow;

        await _unitOfWork.StorageLocations.UpdateAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<bool>> CanAccommodateProductAsync(Guid locationId, int productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (productId <= 0)
            return Result<bool>.Failure("ID do produto inválido");

        if (quantity <= 0)
            return Result<bool>.Failure("Quantidade deve ser maior que 0");

        var canAccommodate = await _unitOfWork.StorageLocations.CanAccommodateProductAsync(locationId, productId, quantity);
        return Result<bool>.Success(canAccommodate);
    }

    public async Task<Result> DeleteAsync(Guid id, string deletedBy, CancellationToken cancellationToken = default)
    {
        var location = await _unitOfWork.StorageLocations.GetByIdAsync(id, cancellationToken);
        if (location == null)
            return Result.Failure("Localização não encontrada");

        await _unitOfWork.StorageLocations.DeleteAsync(location, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<int>> CountAvailableAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.StorageLocations.CountAvailableAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<int>> CountOccupiedAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<int>.Failure("ID do armazém inválido");

        var count = await _unitOfWork.StorageLocations.CountOccupiedAsync(warehouseId);
        return Result<int>.Success(count);
    }

    public async Task<Result<decimal>> GetUtilizationPercentageAsync(int warehouseId, CancellationToken cancellationToken = default)
    {
        if (warehouseId <= 0)
            return Result<decimal>.Failure("ID do armazém inválido");

        var percentage = await _unitOfWork.StorageLocations.GetUtilizationPercentageAsync(warehouseId);
        return Result<decimal>.Success(percentage);
    }

    private static StorageLocationResponse MapToResponse(StorageLocation location)
    {
        return new StorageLocationResponse
        {
            Id = location.Id,
            WarehouseId = location.WarehouseId,
            Code = location.Code,
            Zone = (int)location.Zone,
            Status = (int)location.Status,
            CurrentProductId = location.CurrentProductId,
            CurrentQuantity = location.CurrentQuantity,
            MaxCapacityKg = location.MaxCapacityKg,
            MaxCapacityUnits = location.MaxCapacityUnits,
            RowPosition = location.RowPosition,
            ColumnPosition = location.ColumnPosition,
            LevelPosition = location.LevelPosition,
            CorridorNumber = location.CorridorNumber ?? string.Empty,
            DistanceFromConsolidationPoint = location.DistanceFromConsolidationPoint ?? 0,
            RequiresTemperatureControl = location.RequiresTemperatureControl,
            MinTemperature = location.IdealTemperature,
            MaxTemperature = location.IdealTemperature,
            MinHumidity = location.IdealHumidity,
            MaxHumidity = location.IdealHumidity,
            IsBlocked = location.IsBlocked,
            BlockReason = location.BlockReason,
            AccessCount = location.AccessCount,
            LastRemovalDate = location.LastRemovalDate,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt ?? DateTime.UtcNow
        };
    }
}
