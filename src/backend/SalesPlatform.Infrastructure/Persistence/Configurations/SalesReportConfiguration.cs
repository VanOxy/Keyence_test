using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence.Configurations;

public class SalesReportConfiguration : IEntityTypeConfiguration<SalesReport>
{
    public void Configure(EntityTypeBuilder<SalesReport> builder)
    {
        builder.Property(r => r.OriginalFileName).HasMaxLength(260);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}