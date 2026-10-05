using TaskApi.Domain;

namespace TaskApi.Services;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);

    string CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}