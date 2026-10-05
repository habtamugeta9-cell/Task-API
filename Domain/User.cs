
using TaskApi.Authorization;

namespace TaskApi.Domain;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Email { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; } =
        DateTimeOffset.UtcNow;

    public string? RefreshTokenHash { get; private set; }

    public DateTimeOffset? RefreshTokenExpiresAt { get; private set; }

    public string Role { get; private set; } = Roles.User;

    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
    }

    public User(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email cannot be null or whitespace; it is required.",
                nameof(email));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException(
                "Password hash cannot be null or whitespace; it is required.",
                nameof(passwordHash));
        }

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public void SetRefreshToken(
        string refreshTokenHash,
        DateTimeOffset expiresAt)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }

    public void PromoteToAdmin()
    {
        Role = Roles.Admin;
    }
}
