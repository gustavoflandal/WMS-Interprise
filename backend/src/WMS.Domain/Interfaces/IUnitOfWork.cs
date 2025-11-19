namespace WMS.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Autenticação e Autorização
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IPermissionRepository Permissions { get; }
    ITenantRepository Tenants { get; }

    // Auditoria
    IAuditLogRepository AuditLogs { get; }

    // Dados Mestres
    ICompanyRepository Companies { get; }
    IWarehouseRepository Warehouses { get; }
    ICustomerRepository Customers { get; }

    // Recebimento (RF-001)
    IProductRepository Products { get; }
    IStorageLocationRepository StorageLocations { get; }
    IASNRepository ASNs { get; }
    IReceiptRepository Receipts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
