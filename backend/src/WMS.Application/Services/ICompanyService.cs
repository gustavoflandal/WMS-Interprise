using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface ICompanyService
{
    Task<Result<CompanyResponse?>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<CompanyResponse>> CreateAsync(Guid tenantId, CreateCompanyRequest request, string createdBy, CancellationToken cancellationToken = default);
    Task<Result<CompanyResponse>> UpdateAsync(Guid tenantId, UpdateCompanyRequest request, string updatedBy, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid tenantId, string deletedBy, CancellationToken cancellationToken = default);
}
