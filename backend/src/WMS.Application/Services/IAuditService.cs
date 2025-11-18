using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface IAuditService
{
    Task LogAsync(Guid? userId, string? username, string action, string entityName, string? entityId, string ipAddress, string? userAgent = null, bool isSuccess = true, string? errorMessage = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLogResponse>> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLogResponse>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
