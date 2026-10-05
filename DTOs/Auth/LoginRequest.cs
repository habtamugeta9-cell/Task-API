namespace TaskApi.DTO.Auth;

public sealed record LoginRequest(
    string Email,
    string Password);