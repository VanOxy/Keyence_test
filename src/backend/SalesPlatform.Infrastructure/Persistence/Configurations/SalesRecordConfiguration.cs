using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence.Configurations;

public class SalesRecordConfiguration : IEntityTypeConfiguration<SalesRecord>
{
    public void Configure(EntityTypeBuilder<SalesRecord> builder)
    {
        builder.Property(r => r.CompanyName).HasMaxLength(200);
        builder.Property(r => r.ContactPerson).HasMaxLength(150);
        builder.Property(r => r.Phone).HasMaxLength(30);
        builder.Property(r => r.Email).HasMaxLength(256);
        builder.Property(r => r.Region).HasMaxLength(50);
        builder.Property(r => r.Product).HasMaxLength(100);
        builder.Property(r => r.UnitPrice).HasPrecision(18, 2);
        builder.Property(r => r.Revenue).HasPrecision(18, 2);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
