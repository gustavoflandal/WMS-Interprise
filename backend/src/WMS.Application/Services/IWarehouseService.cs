using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface IWarehouseService
{
    Task<Result<IEnumerable<WarehouseResponse>>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<WarehouseResponse?>> GetByIdAsync(Guid tenantId, Guid id, CancellationToken cancellationToken = default);
    Task<Result<WarehouseResponse>> CreateAsync(Guid tenantId, CreateWarehouseRequest request, Guid? createdBy, CancellationToken cancellationToken = default);
    Task<Result<WarehouseResponse>> UpdateAsync(Guid tenantId, Guid id, UpdateWarehouseRequest request, Guid? updatedBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid tenantId, Guid id, Guid? deletedBy, CancellationToken cancellationToken = default);
}
