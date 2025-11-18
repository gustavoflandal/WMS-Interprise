namespace WMS.Application.DTOs.Requests;

public record LoginRequest(
    string Username,
    string Password,
    bool RememberMe = false
);
