using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
