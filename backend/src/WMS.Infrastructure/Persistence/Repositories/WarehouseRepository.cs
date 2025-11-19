using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories;

public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Warehouse>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(w => w.TenantId == tenantId && !w.IsDeleted)
            .OrderBy(w => w.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Warehouse?> GetByCodeAsync(Guid tenantId, string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(w => w.TenantId == tenantId && w.Codigo == codigo && !w.IsDeleted, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(Guid tenantId, string codigo, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(w => w.TenantId == tenantId && w.Codigo == codigo && !w.IsDeleted, cancellationToken);
    }

    public async Task<bool> CodeExistsForOtherWarehouseAsync(Guid tenantId, string codigo, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(w => w.TenantId == tenantId && w.Codigo == codigo && w.Id != warehouseId && !w.IsDeleted, cancellationToken);
    }
}
