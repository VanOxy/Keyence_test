using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SalesPlatform.Application.Auth.Commands.Login;
using SalesPlatform.Domain.Entities;
using SalesPlatform.Domain.Enums;
using SalesPlatform.Infrastructure.Identity;
using SalesPlatform.Infrastructure.Persistence;

namespace SalesPlatform.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private const string SeedPassword = "Passw0rd!";

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static JwtTokenGenerator CreateJwtTokenGenerator()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "test-secret-test-secret-test-secret-32b",
                ["Jwt:Issuer"] = "SalesPlatformTests",
                ["Jwt:Audience"] = "SalesPlatformTests",
                ["Jwt:ExpiryMinutes"] = "15"
            })
            .Build();

        return new JwtTokenGenerator(configuration);
    }

    private static async Task<User> SeedUser(AppDbContext db, PasswordHasher passwordHasher)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = "Test User",
            Email = "test.user@salesplatform.local",
            PasswordHash = passwordHasher.Hash(SeedPassword),
            Role = UserRole.SalesRep,
            Country = "HU",
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsTokenAndUserInfo()
    {
        using var db = CreateDb();
        var passwordHasher = new PasswordHasher();
        var user = await SeedUser(db, passwordHasher);
        var handler = new LoginCommandHandler(db, passwordHasher, CreateJwtTokenGenerator());

        var result = await handler.Handle(new LoginCommand(user.Email, SeedPassword), CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Role.ToString(), result.Role);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ThrowsUnauthorizedAccessException()
    {
        using var db = CreateDb();
        var passwordHasher = new PasswordHasher();
        var user = await SeedUser(db, passwordHasher);
        var handler = new LoginCommandHandler(db, passwordHasher, CreateJwtTokenGenerator());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new LoginCommand(user.Email, "wrong-password"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ThrowsUnauthorizedAccessException()
    {
        using var db = CreateDb();
        var passwordHasher = new PasswordHasher();
        await SeedUser(db, passwordHasher);
        var handler = new LoginCommandHandler(db, passwordHasher, CreateJwtTokenGenerator());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            handler.Handle(new LoginCommand("nobody@salesplatform.local", SeedPassword), CancellationToken.None));
    }
}
