namespace WMS.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Permission() { } // For EF Core

    public Permission(
        string name,
        string resource,
        string action,
        string description,
        string module)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        Action = action ?? throw new ArgumentNullException(nameof(action));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Module = module ?? throw new ArgumentNullException(nameof(module));
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string GetPermissionCode() => $"{Resource}.{Action}";
}
