using System.Security.Claims;
using TaskApi.Authorization;

namespace TaskApi.Services;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException(
            "No active HTTP context is available.");

    public bool IsAuthenticated =>
        User.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (claim is null ||
                !Guid.TryParse(claim.Value, out var userId))
            {
                throw new InvalidOperationException(
                    "Authenticated user ID is missing.");
            }

            return userId;
        }
    }

    public string? Email =>
        User.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role =>
        User.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAdmin =>
        User.IsInRole(Roles.Admin);
}
