using Microsoft.Extensions.Configuration;
using WMS.Application.Common;
using WMS.Application.DTOs.Requests;
using WMS.Application.DTOs.Responses;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;
using WMS.Infrastructure.Security;
using WMS.Shared.Interfaces;

namespace WMS.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IPasswordService passwordService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _configuration = configuration;
    }

    public async Task<Result<AuthenticationResponse>> LoginAsync(
        LoginRequest request,
        string ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        // Find user by username or email
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username, cancellationToken)
            ?? await _unitOfWork.Users.GetByEmailAsync(request.Username, cancellationToken);

        if (user == null)
            return Result<AuthenticationResponse>.Failure("Invalid username or password");

        // Check if user is locked out
        if (user.IsLockedOut())
            return Result<AuthenticationResponse>.Failure("Account is locked. Please try again later");

        // Verify password
        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<AuthenticationResponse>.Failure("Invalid username or password");
        }

        // Check if user is active
        if (!user.IsActive)
            return Result<AuthenticationResponse>.Failure("User account is inactive");

        // Load user with roles
        user = await _unitOfWork.Users.GetWithRolesAsync(user.Id, cancellationToken) ?? user;

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expirationMinutes = _configuration.GetValue("JwtSettings:ExpirationMinutes", 60);

        // Update refresh token
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordSuccessfulLogin();

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rolesList = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>();

        var response = new AuthenticationResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(expirationMinutes),
            User: new UserResponse(
                Id: user.Id,
                Username: user.Username,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Phone: user.Phone,
                IsActive: user.IsActive,
                EmailConfirmed: user.EmailConfirmed,
                LastLoginAt: user.LastLoginAt,
                CreatedAt: user.CreatedAt,
                Roles: rolesList,
                Permissions: new List<string>()
            )
        );

        return Result<AuthenticationResponse>.Success(response);
    }

    public async Task<Result<AuthenticationResponse>> RegisterAsync(
        RegisterUserRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Result<AuthenticationResponse>.Failure("Username, email and password are required");

        // Validate password confirmation
        if (request.Password != request.ConfirmPassword)
            return Result<AuthenticationResponse>.Failure("Passwords do not match");

        // Check if user already exists
        if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
            return Result<AuthenticationResponse>.Failure("Username already exists");

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            return Result<AuthenticationResponse>.Failure("Email already exists");

        // Create user
        var passwordHash = _passwordService.HashPassword(request.Password);
        var newUser = new User(
            username: request.Username,
            email: request.Email,
            passwordHash: passwordHash,
            firstName: request.FirstName ?? "User",
            lastName: request.LastName ?? "",
            phone: request.Phone
        );

        await _unitOfWork.Users.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(newUser);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var expirationMinutes = _configuration.GetValue("JwtSettings:ExpirationMinutes", 60);

        newUser.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        await _unitOfWork.Users.UpdateAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new AuthenticationResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(expirationMinutes),
            User: new UserResponse(
                Id: newUser.Id,
                Username: newUser.Username,
                Email: newUser.Email,
                FirstName: newUser.FirstName,
                LastName: newUser.LastName,
                Phone: newUser.Phone,
                IsActive: newUser.IsActive,
                EmailConfirmed: newUser.EmailConfirmed,
                LastLoginAt: newUser.LastLoginAt,
                CreatedAt: newUser.CreatedAt,
                Roles: new List<string>(),
                Permissions: new List<string>()
            )
        );

        return Result<AuthenticationResponse>.Success(response);
    }

    public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Result<AuthenticationResponse>.Failure("Invalid or expired refresh token");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var expirationMinutes = _configuration.GetValue("JwtSettings:ExpirationMinutes", 60);

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rolesList = user.UserRoles?.Select(ur => ur.Role?.Name ?? "").ToList() ?? new List<string>();

        var response = new AuthenticationResponse(
            AccessToken: accessToken,
            RefreshToken: newRefreshToken,
            ExpiresAt: DateTime.UtcNow.AddMinutes(expirationMinutes),
            User: new UserResponse(
                Id: user.Id,
                Username: user.Username,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Phone: user.Phone,
                IsActive: user.IsActive,
                EmailConfirmed: user.EmailConfirmed,
                LastLoginAt: user.LastLoginAt,
                CreatedAt: user.CreatedAt,
                Roles: rolesList,
                Permissions: new List<string>()
            )
        );

        return Result<AuthenticationResponse>.Success(response);
    }

    public async Task<Result> LogoutAsync(Guid userId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        user.RevokeRefreshToken();
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return Result.Failure("User not found");

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect");

        var newPasswordHash = _passwordService.HashPassword(request.NewPassword);
        user.UpdatePassword(newPasswordHash);
        user.UpdateTimestamp("User");

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
