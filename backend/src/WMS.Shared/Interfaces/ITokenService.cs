namespace WMS.Shared.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(WMS.Domain.Entities.User user);
    string GenerateRefreshToken();
}
