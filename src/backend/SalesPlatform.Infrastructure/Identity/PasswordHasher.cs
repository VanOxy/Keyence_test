using Microsoft.AspNetCore.Identity;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _inner = new();

    public string Hash(string password) =>
        _inner.HashPassword(null!, password);

    public bool Verify(string password, string hash) =>
        _inner.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
