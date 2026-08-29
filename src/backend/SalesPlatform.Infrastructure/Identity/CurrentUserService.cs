using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No active HTTP request to read the current user from.");

    public Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public UserRole Role => Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role)!);
}
