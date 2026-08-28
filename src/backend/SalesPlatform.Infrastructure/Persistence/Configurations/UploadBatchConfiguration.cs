using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence.Configurations;

public class UploadBatchConfiguration : IEntityTypeConfiguration<UploadBatch>
{
    public void Configure(EntityTypeBuilder<UploadBatch> builder)
    {
        builder.Property(b => b.OriginalFileName).HasMaxLength(260);
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UploadedByUserId);
    }
}
