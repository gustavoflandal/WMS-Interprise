namespace WMS.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public string? AssignedBy { get; private set; }

    // Navigation properties
    public virtual Role Role { get; private set; } = null!;
    public virtual Permission Permission { get; private set; } = null!;

    private RolePermission() { } // For EF Core

    public RolePermission(Guid roleId, Guid permissionId, string? assignedBy = null)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }
}
