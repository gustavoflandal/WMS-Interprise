namespace WMS.Application.DTOs.Responses;

public record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserResponse User
);
