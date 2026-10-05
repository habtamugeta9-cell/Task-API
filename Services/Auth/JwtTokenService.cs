using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskApi.Domain;

namespace TaskApi.Services.Auth;

public sealed class JwtTokenService(
    IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions jwtOptions = options.Value;

    public string CreateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Role,
                user.Role),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                jwtOptions.AccessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public string CreateRefreshToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashRefreshToken(
        string refreshToken)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(refreshToken));

        return Convert.ToHexString(bytes);
    }
}