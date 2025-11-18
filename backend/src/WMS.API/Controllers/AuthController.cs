using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WMS.Application.DTOs.Requests;
using WMS.Application.Services;

namespace WMS.API.Controllers;

/// <summary>
/// Endpoints de autenticação e autorização
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém o endereço IP do cliente
    /// </summary>
    private string GetClientIpAddress()
    {
        if (HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.ToString().Split(',')[0].Trim();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    /// <summary>
    /// Realiza login de um usuário
    /// </summary>
    /// <param name="request">Credenciais de login (username/email e password)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token de acesso e refresh token</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = GetClientIpAddress();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
        var result = await _authService.LoginAsync(request, ipAddress, userAgent, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Login failed for user: {Username}. Reason: {Error}", request.Username, result.Error);
            return Unauthorized(new { message = result.Error });
        }

        _logger.LogInformation("Login successful for user: {Username}", request.Username);
        return Ok(result.Value);
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    /// <param name="request">Dados de registro (username, email, password)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do usuário criado</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Register attempt for username: {Username}", request.Username);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = GetClientIpAddress();
        var result = await _authService.RegisterAsync(request, ipAddress, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Registration failed for username: {Username}. Reason: {Error}", request.Username, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Registration successful for username: {Username}", request.Username);
        return CreatedAtAction(nameof(Register), result.Value);
    }

    /// <summary>
    /// Atualiza o token de acesso usando um refresh token válido
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Novo token de acesso</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token attempt");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var ipAddress = GetClientIpAddress();
        var result = await _authService.RefreshTokenAsync(request, ipAddress, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Refresh token failed. Reason: {Error}", result.Error);
            return Unauthorized(new { message = result.Error });
        }

        _logger.LogInformation("Token refreshed successfully");
        return Ok(result.Value);
    }

    /// <summary>
    /// Realiza logout do usuário atual
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logout attempt for user: {UserId}", User.FindFirst("sub")?.Value ?? "unknown");

        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var ipAddress = GetClientIpAddress();
        var result = await _authService.LogoutAsync(userId, ipAddress, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Logout failed. Reason: {Error}", result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Logout successful for user: {UserId}", userId);
        return Ok(new { message = "Logout successful" });
    }

    /// <summary>
    /// Altera a senha do usuário autenticado
    /// </summary>
    /// <param name="request">Senha atual e nova senha</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Confirmação de mudança de senha</returns>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Change password attempt for user: {UserId}", User.FindFirst("sub")?.Value ?? "unknown");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var ipAddress = GetClientIpAddress();
        var result = await _authService.ChangePasswordAsync(userId, request, ipAddress, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Change password failed for user: {UserId}. Reason: {Error}", userId, result.Error);
            return BadRequest(new { message = result.Error });
        }

        _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
        return Ok(new { message = "Password changed successfully" });
    }
}

