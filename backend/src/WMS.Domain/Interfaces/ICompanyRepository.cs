using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Company?> GetByCNPJAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<bool> CNPJExistsAsync(string cnpj, CancellationToken cancellationToken = default);
}
