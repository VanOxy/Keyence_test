using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

public class GetSalesRecordsListQueryHandler : IRequestHandler<GetSalesRecordsListQuery, IReadOnlyList<SalesRecordListItem>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSalesRecordsListQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<SalesRecordListItem>> Handle(GetSalesRecordsListQuery request, CancellationToken cancellationToken)
    {
        var query = _db.SalesRecords.Where(r => r.OwnerUserId == _currentUser.UserId);

        if (request.SalesReportId is { } salesReportId)
            query = query.Where(r => r.SalesReportId == salesReportId);

        return await query
            .OrderByDescending(r => r.Date)
            .Select(r => new SalesRecordListItem(
                r.Id,
                r.Date,
                r.CompanyName,
                r.ContactPerson,
                r.Phone,
                r.Email,
                r.Region,
                r.Product,
                r.UnitsSold,
                r.UnitPrice,
                r.Revenue))
            .ToListAsync(cancellationToken);
    }
}
