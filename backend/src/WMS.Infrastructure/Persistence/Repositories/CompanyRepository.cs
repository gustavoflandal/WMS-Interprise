using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories;

public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
{
    public CompanyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Company?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.TenantId == tenantId && !c.IsDeleted, cancellationToken);
    }

    public async Task<Company?> GetByCNPJAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.CNPJ == cnpj && !c.IsDeleted, cancellationToken);
    }

    public async Task<bool> CNPJExistsAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.CNPJ == cnpj && !c.IsDeleted, cancellationToken);
    }
}
