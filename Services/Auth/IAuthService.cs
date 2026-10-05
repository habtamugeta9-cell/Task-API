using TaskApi.DTO.Auth;

namespace TaskApi.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse?> LoginAsync(LoginRequest request);

    Task<AuthResponse?> RefreshAsync(RefreshRequest request);
}