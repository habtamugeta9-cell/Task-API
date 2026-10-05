using TaskApi.Domain;

namespace TaskApi.Services.Auth;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);

    string CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}