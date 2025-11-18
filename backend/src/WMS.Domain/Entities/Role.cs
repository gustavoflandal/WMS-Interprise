namespace WMS.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsSystemRole { get; private set; }
    public Guid? TenantId { get; private set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    public virtual Tenant? Tenant { get; private set; }

    private Role() { } // For EF Core

    public Role(string name, string description, bool isSystemRole = false, Guid? tenantId = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        IsSystemRole = isSystemRole;
        TenantId = tenantId;
    }

    public void Update(string name, string description)
    {
        if (IsSystemRole)
        {
            throw new InvalidOperationException("Cannot modify system roles");
        }

        Name = name;
        Description = description;
    }

    public void AddPermission(Permission permission)
    {
        if (!RolePermissions.Any(rp => rp.PermissionId == permission.Id))
        {
            RolePermissions.Add(new RolePermission(Id, permission.Id));
        }
    }

    public void RemovePermission(Guid permissionId)
    {
        if (IsSystemRole)
        {
            throw new InvalidOperationException("Cannot modify permissions of system roles");
        }

        var rolePermission = RolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            RolePermissions.Remove(rolePermission);
        }
    }
}
