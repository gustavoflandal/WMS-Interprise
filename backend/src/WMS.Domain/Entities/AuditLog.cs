namespace WMS.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string? Username { get; private set; }
    public Guid? TenantId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? Changes { get; private set; }
    public string IpAddress { get; private set; } = string.Empty;
    public string? UserAgent { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string? AdditionalData { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    private AuditLog()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }

    public AuditLog(
        Guid? userId,
        string? username,
        Guid? tenantId,
        string action,
        string entityName,
        string? entityId,
        string ipAddress,
        string? userAgent = null,
        bool isSuccess = true,
        string? errorMessage = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Username = username;
        TenantId = tenantId;
        Action = action ?? throw new ArgumentNullException(nameof(action));
        EntityName = entityName ?? throw new ArgumentNullException(nameof(entityName));
        EntityId = entityId;
        IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        UserAgent = userAgent;
        Timestamp = DateTime.UtcNow;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public void SetChanges(string? oldValues, string? newValues, string? changes)
    {
        OldValues = oldValues;
        NewValues = newValues;
        Changes = changes;
    }

    public void SetAdditionalData(string additionalData)
    {
        AdditionalData = additionalData;
    }

    public static AuditLog CreateLoginLog(
        Guid? userId,
        string? username,
        string ipAddress,
        string? userAgent,
        bool isSuccess,
        string? errorMessage = null)
    {
        return new AuditLog(
            userId,
            username,
            null,
            "Login",
            "User",
            userId?.ToString(),
            ipAddress,
            userAgent,
            isSuccess,
            errorMessage);
    }

    public static AuditLog CreateLogoutLog(
        Guid userId,
        string username,
        string ipAddress,
        string? userAgent)
    {
        return new AuditLog(
            userId,
            username,
            null,
            "Logout",
            "User",
            userId.ToString(),
            ipAddress,
            userAgent,
            true);
    }

    public static AuditLog CreateEntityLog(
        Guid userId,
        string username,
        Guid? tenantId,
        string action,
        string entityName,
        string entityId,
        string ipAddress,
        string? userAgent = null)
    {
        return new AuditLog(
            userId,
            username,
            tenantId,
            action,
            entityName,
            entityId,
            ipAddress,
            userAgent,
            true);
    }
}
