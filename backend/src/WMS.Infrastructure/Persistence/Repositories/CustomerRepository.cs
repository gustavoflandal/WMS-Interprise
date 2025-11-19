using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .OrderBy(c => c.Nome)
            .ToListAsync(cancellationToken);
    }

    public async Task<Customer?> GetByDocumentoAsync(Guid tenantId, string numeroDocumento)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.NumeroDocumento == numeroDocumento && !c.IsDeleted);
    }

    public async Task<IEnumerable<Customer>> GetByStatusAsync(Guid tenantId, CustomerStatus status)
    {
        return await _dbSet
            .Where(c => c.TenantId == tenantId && c.Status == status && !c.IsDeleted)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<bool> DocumentoExistsAsync(Guid tenantId, string numeroDocumento, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.TenantId == tenantId && c.NumeroDocumento == numeroDocumento && !c.IsDeleted);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
