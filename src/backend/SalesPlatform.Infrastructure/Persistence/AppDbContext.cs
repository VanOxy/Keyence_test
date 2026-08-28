using Microsoft.EntityFrameworkCore;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UploadBatch> UploadBatches => Set<UploadBatch>();
    public DbSet<SalesRecord> SalesRecords => Set<SalesRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}