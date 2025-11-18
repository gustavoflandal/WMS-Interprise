using Microsoft.Extensions.Logging;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Application.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IUnitOfWork unitOfWork, ILogger<AuditService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogAsync(
        Guid? userId,
        string? username,
        string action,
        string entityName,
        string? entityId,
        string ipAddress,
        string? userAgent = null,
        bool isSuccess = true,
        string? errorMessage = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog(
                userId: userId,
                username: username ?? "anonymous",
                tenantId: null,  // Should be populated from context
                action: action,
                entityName: entityName,
                entityId: entityId,
                ipAddress: ipAddress,
                userAgent: userAgent,
                isSuccess: isSuccess,
                errorMessage: errorMessage
            );

            await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit: {Action} on {EntityName}", action, entityName);
        }
    }

    public async Task<IEnumerable<AuditLogResponse>> GetByUserIdAsync(
        Guid userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new List<AuditLogResponse>();
        }

        var auditLogs = await _unitOfWork.AuditLogs.GetByUserIdAsync(userId, pageNumber, pageSize, cancellationToken);

        return auditLogs.Select(al => new AuditLogResponse(
            Id: al.Id,
            Username: al.Username,
            Action: al.Action,
            EntityName: al.EntityName,
            EntityId: al.EntityId,
            Changes: al.Changes,
            IpAddress: al.IpAddress,
            Timestamp: al.Timestamp,
            IsSuccess: al.IsSuccess,
            ErrorMessage: al.ErrorMessage
        )).ToList();
    }

    public async Task<IEnumerable<AuditLogResponse>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var auditLogs = await _unitOfWork.AuditLogs.GetByDateRangeAsync(startDate, endDate, cancellationToken);

        return auditLogs.Select(al => new AuditLogResponse(
            Id: al.Id,
            Username: al.Username,
            Action: al.Action,
            EntityName: al.EntityName,
            EntityId: al.EntityId,
            Changes: al.Changes,
            IpAddress: al.IpAddress,
            Timestamp: al.Timestamp,
            IsSuccess: al.IsSuccess,
            ErrorMessage: al.ErrorMessage
        )).ToList();
    }
}
