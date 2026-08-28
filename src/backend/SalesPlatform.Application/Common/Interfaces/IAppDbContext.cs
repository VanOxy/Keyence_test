using Microsoft.EntityFrameworkCore;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<UploadBatch> UploadBatches { get; }
    DbSet<SalesRecord> SalesRecords { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
