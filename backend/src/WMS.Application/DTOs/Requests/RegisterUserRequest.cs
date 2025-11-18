namespace WMS.Application.DTOs.Requests;

public record RegisterUserRequest(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string? Phone = null
);
