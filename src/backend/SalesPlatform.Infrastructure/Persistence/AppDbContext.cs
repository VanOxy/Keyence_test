using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<SalesReport> SalesReports => Set<SalesReport>();
    public DbSet<SalesRecord> SalesRecords => Set<SalesRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}