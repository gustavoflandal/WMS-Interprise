using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;

namespace WMS.Application.Services;

public interface IAuthenticationService
{
    Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request, string ipAddress, string? userAgent, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RegisterAsync(RegisterUserRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, string ipAddress, CancellationToken cancellationToken = default);
}
