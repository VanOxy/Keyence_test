using System.Linq.Dynamic.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

public class GetSalesRecordsListQueryHandler : IRequestHandler<GetSalesRecordsListQuery, SalesRecordsPage>
{
    private static readonly HashSet<string> SortableFields = new(StringComparer.OrdinalIgnoreCase)
    {
        nameof(SalesRecordListItem.Date),
        nameof(SalesRecordListItem.CompanyName),
        nameof(SalesRecordListItem.ContactPerson),
        nameof(SalesRecordListItem.Phone),
        nameof(SalesRecordListItem.Email),
        nameof(SalesRecordListItem.Region),
        nameof(SalesRecordListItem.Product),
        nameof(SalesRecordListItem.UnitsSold),
        nameof(SalesRecordListItem.UnitPrice),
        nameof(SalesRecordListItem.Revenue),
    };

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSalesRecordsListQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<SalesRecordsPage> Handle(GetSalesRecordsListQuery request, CancellationToken cancellationToken)
    {
        var query = _db.SalesRecords.AsQueryable();

        if (_currentUser.Role == UserRole.SalesRep)
            query = query.Where(r => r.OwnerUserId == _currentUser.UserId);

        if (request.SalesReportId is { } salesReportId)
            query = query.Where(r => r.SalesReportId == salesReportId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(r =>
                r.CompanyName.Contains(search) ||
                r.ContactPerson.Contains(search) ||
                r.Product.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy is not null && SortableFields.Contains(request.SortBy)
            ? query.OrderBy($"{request.SortBy} {(request.Descending ? "descending" : "ascending")}")
            : query.OrderByDescending(r => r.Date);

        var page = await query
            .Skip(request.Skip)
            .Take(request.Take)
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

        return new SalesRecordsPage(page, totalCount);
    }
}