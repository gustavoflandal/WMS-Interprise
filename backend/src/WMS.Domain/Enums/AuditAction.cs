namespace WMS.Domain.Enums;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Read = 4,
    Login = 5,
    Logout = 6,
    PasswordChange = 7,
    PasswordReset = 8,
    RoleAssigned = 9,
    RoleRemoved = 10,
    PermissionGranted = 11,
    PermissionRevoked = 12,
    Export = 13,
    Import = 14,
    Download = 15,
    Upload = 16
}
