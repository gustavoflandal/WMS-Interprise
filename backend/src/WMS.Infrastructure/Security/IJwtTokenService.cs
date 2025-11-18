using WMS.Domain.Entities;

namespace WMS.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
