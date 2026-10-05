namespace TaskApi.DTO.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken);