namespace WMS.Application.DTOs.Responses;

public record UserResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    bool IsActive,
    bool EmailConfirmed,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    List<string> Roles,
    List<string> Permissions
);
