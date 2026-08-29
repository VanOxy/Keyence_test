using Microsoft.EntityFrameworkCore;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<SalesReport> SalesReports { get; }
    DbSet<SalesRecord> SalesRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
