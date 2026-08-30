using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;

namespace SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

public class GetSalesReportListQueryHandler : IRequestHandler<GetSalesReportListQuery, IReadOnlyList<SalesReportListItem>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSalesReportListQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SalesReportListItem>> Handle(GetSalesReportListQuery request, CancellationToken cancellationToken)
    {
        return await _db.SalesReports
            .Where(r => r.OwnerUserId == _currentUser.UserId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Select(r => new SalesReportListItem(r.Id, r.OriginalFileName, r.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
