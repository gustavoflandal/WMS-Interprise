using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IWarehouseRepository : IRepository<Warehouse>
{
    Task<IEnumerable<Warehouse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Warehouse?> GetByCodeAsync(Guid tenantId, string codigo, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(Guid tenantId, string codigo, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsForOtherWarehouseAsync(Guid tenantId, string codigo, Guid warehouseId, CancellationToken cancellationToken = default);
}
