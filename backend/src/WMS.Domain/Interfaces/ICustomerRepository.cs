namespace WMS.Domain.Interfaces;

using WMS.Domain.Entities;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Customer?> GetByDocumentoAsync(Guid tenantId, string numeroDocumento);
    Task<IEnumerable<Customer>> GetByStatusAsync(Guid tenantId, CustomerStatus status);
    Task<bool> DocumentoExistsAsync(Guid tenantId, string numeroDocumento, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
