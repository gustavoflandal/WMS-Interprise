using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface ICustomerService
{
    Task<Result<List<CustomerResponse>>> GetAllByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse?>> GetByIdAsync(Guid tenantId, Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> CreateAsync(Guid tenantId, CreateCustomerRequest request, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> UpdateAsync(Guid tenantId, Guid customerId, UpdateCustomerRequest request, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid tenantId, Guid customerId, Guid? userId = null, CancellationToken cancellationToken = default);
}
