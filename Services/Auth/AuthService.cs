using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskApi.Data;
using TaskApi.Domain;
using TaskApi.DTO.Auth;

namespace TaskApi.Services.Auth;

public sealed class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService,
    IOptions<JwtOptions> jwtOptions,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);

        ValidateRegistration(request.Password);

        var existingUser = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        var userForPasswordHash = new User(
            email,
            "pending-password-hash");

        var passwordHash = passwordHasher.HashPassword(
            userForPasswordHash,
            request.Password);

        var user = new User(
            email,
            passwordHash);

        var refreshToken =
            jwtTokenService.CreateRefreshToken();

        var refreshTokenHash =
            jwtTokenService.HashRefreshToken(refreshToken);

        user.SetRefreshToken(
            refreshTokenHash,
            DateTime.UtcNow.AddDays(
                jwtOptions.Value.RefreshTokenDays));

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "User registered. UserId: {UserId}",
            user.Id);

        return CreateAuthResponse(
            user,
            refreshToken);
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                candidate => candidate.Email == email);

        if (user is null)
        {
            return null;
        }

        var verificationResult =
            passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verificationResult ==
            PasswordVerificationResult.Failed)
        {
            return null;
        }

        var refreshToken =
            jwtTokenService.CreateRefreshToken();

        var refreshTokenHash =
            jwtTokenService.HashRefreshToken(refreshToken);

        user.SetRefreshToken(
            refreshTokenHash,
            DateTime.UtcNow.AddDays(
                jwtOptions.Value.RefreshTokenDays));

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "User logged in. UserId: {UserId}",
            user.Id);

        return CreateAuthResponse(
            user,
            refreshToken);
    }

    public async Task<AuthResponse?> RefreshAsync(
        RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return null;
        }

        var refreshTokenHash =
            jwtTokenService.HashRefreshToken(
                request.RefreshToken);

        var user = await dbContext.Users
            .FirstOrDefaultAsync(candidate =>
                candidate.RefreshTokenHash == refreshTokenHash);

        if (user is null ||
            user.RefreshTokenExpiresAt is null ||
            user.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var newRefreshToken =
            jwtTokenService.CreateRefreshToken();

        var newRefreshTokenHash =
            jwtTokenService.HashRefreshToken(
                newRefreshToken);

        user.SetRefreshToken(
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(
                jwtOptions.Value.RefreshTokenDays));

        await dbContext.SaveChangesAsync();

        logger.LogInformation(
            "Refresh token rotated. UserId: {UserId}",
            user.Id);

        return CreateAuthResponse(
            user,
            newRefreshToken);
    }

    private AuthResponse CreateAuthResponse(
        User user,
        string refreshToken)
    {
        var accessToken =
            jwtTokenService.CreateAccessToken(user);

        return new AuthResponse(
            accessToken,
            refreshToken);
    }

    private static string NormalizeEmail(
        string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(
                "Email is required.",
                nameof(email));
        }

        return email.Trim().ToLowerInvariant();
    }

    private static void ValidateRegistration(
        string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "Password is required.",
                nameof(password));
        }

        if (password.Length < 8)
        {
            throw new ArgumentException(
                "Password must contain at least 8 characters.",
                nameof(password));
        }
    }
}