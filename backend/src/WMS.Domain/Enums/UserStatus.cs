namespace WMS.Domain.Enums;

public enum UserStatus
{
    Active = 1,
    Inactive = 2,
    LockedOut = 3,
    PendingEmailConfirmation = 4,
    Suspended = 5
}
