using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Entities;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Infrastructure.Persistence;

public static class DbSeeder
{
    private const string SeedPassword = "p@ssw0rd*-";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher passwordHasher)
    {
        if (await db.Users.AnyAsync())
            return;

        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Anna Kovacs",
                Email = "anna.kovacs@test.com",
                PasswordHash = passwordHasher.Hash(SeedPassword),
                Role = UserRole.SalesRep,
                Country = "HU",
                CreatedAtUtc = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Jan Kowalski",
                Email = "jan.kowalski@test.com",
                PasswordHash = passwordHasher.Hash(SeedPassword),
                Role = UserRole.SalesRep,
                Country = "PL",
                CreatedAtUtc = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Maria Nagy",
                Email = "manager@test.com",
                PasswordHash = passwordHasher.Hash(SeedPassword),
                Role = UserRole.Manager,
                Country = "HU",
                CreatedAtUtc = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "admin@test.com",
                PasswordHash = passwordHasher.Hash(SeedPassword),
                Role = UserRole.Admin,
                Country = "HU",
                CreatedAtUtc = DateTime.UtcNow
            }
        };

        db.Users.AddRange(users);
        await db.SaveChangesAsync();
    }
}