using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FullName).HasMaxLength(100);
        builder.Property(u => u.Email).HasMaxLength(100);
        builder.Property(u => u.Country).HasMaxLength(100);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}