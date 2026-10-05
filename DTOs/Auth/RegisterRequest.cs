namespace TaskApi.DTO.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password);