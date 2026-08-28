using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}
