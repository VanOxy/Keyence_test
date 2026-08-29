using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    UserRole Role { get; }
}
