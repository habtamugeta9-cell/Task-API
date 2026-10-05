


namespace TaskApi.Domain;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public String Email { get; private set; }
    public String PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public String? RefreshTokenHash { get; private set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; private set; }

    private User()
    {
        Email = String.Empty;
        PasswordHash = String.Empty;
    }

    public User(String email, String passwordHash)
    {
        if (String.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be null or whitespace; it is required.", nameof(email));
        }
        if (String.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("Password hash cannot be null or whitespace; it is required.", nameof(passwordHash));
        }

        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public void SetRefreshToken(String refreshTokenHash, DateTimeOffset expiresAt)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }

}
