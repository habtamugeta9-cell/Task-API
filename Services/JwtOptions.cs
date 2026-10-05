namespace TaskApi.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Key { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int AccessTokenMinutes { get; init; }
    public int RefreshTokenDays { get; init; }
}