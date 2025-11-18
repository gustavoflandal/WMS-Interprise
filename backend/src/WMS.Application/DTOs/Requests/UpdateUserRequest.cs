namespace WMS.Application.DTOs.Requests;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? Phone = null,
    bool? IsActive = null
);
