namespace WMS.Application.DTOs.Requests;

public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone = null,
    List<Guid>? RoleIds = null,
    bool IsActive = true
);
