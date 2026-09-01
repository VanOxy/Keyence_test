using System.Linq.Dynamic.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

public class GetSalesReportListQueryHandler : IRequestHandler<GetSalesReportListQuery, SalesReportsPage>
{
    // Maps a public SalesReportListItem property name to where that value actually lives in the
    // join below. EF Core can't translate "ORDER BY" applied *after* a Select(x => new SomeRecord(...))
    // projection (works fine for anonymous types, not for a record's constructor) — so we sort the
    // join first, on its own shape, and project into SalesReportListItem only after Skip/Take.
    private static readonly Dictionary<string, string> SortPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(SalesReportListItem.OriginalFileName)] = "Report.OriginalFileName",
        [nameof(SalesReportListItem.CreatedAtUtc)] = "Report.CreatedAtUtc",
        [nameof(SalesReportListItem.OwnerFullName)] = "OwnerFullName",
    };

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSalesReportListQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<SalesReportsPage> Handle(GetSalesReportListQuery request, CancellationToken cancellationToken)
    {
        var query =
            from r in _db.SalesReports
            join u in _db.Users on r.OwnerUserId equals u.Id
            select new { Report = r, OwnerFullName = u.FullName };

        // SalesRep only ever sees their own reports. Manager/Admin always see everyone's —
        // if they want just their own, they search their own name.
        if (_currentUser.Role == UserRole.SalesRep)
            query = query.Where(x => x.Report.OwnerUserId == _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                x.Report.OriginalFileName.Contains(search) ||
                x.OwnerFullName.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy is not null && SortPaths.TryGetValue(request.SortBy, out var sortPath)
            ? query.OrderBy($"{sortPath} {(request.Descending ? "descending" : "ascending")}")
            : query.OrderByDescending(x => x.Report.CreatedAtUtc);

        var page = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(x => new SalesReportListItem(x.Report.Id, x.Report.OriginalFileName, x.Report.CreatedAtUtc, x.Report.OwnerUserId, x.OwnerFullName))
            .ToListAsync(cancellationToken);

        return new SalesReportsPage(page, totalCount);
    }
}