namespace WMS.Application.DTOs.Responses;

public record AuditLogResponse(
    Guid Id,
    string? Username,
    string Action,
    string EntityName,
    string? EntityId,
    string? Changes,
    string IpAddress,
    DateTime Timestamp,
    bool IsSuccess,
    string? ErrorMessage
);
