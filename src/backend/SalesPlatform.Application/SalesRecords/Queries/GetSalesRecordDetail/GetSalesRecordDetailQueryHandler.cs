using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordDetail;

public class GetSalesRecordDetailQueryHandler : IRequestHandler<GetSalesRecordDetailQuery, SalesRecordListItem?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetSalesRecordDetailQueryHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<SalesRecordListItem?> Handle(GetSalesRecordDetailQuery request, CancellationToken cancellationToken)
    {
        var query = _db.SalesRecords.Where(r => r.Id == request.Id);

        // SalesRep can only load their own record for editing; Manager/Admin can load anyone's.
        if (_currentUser.Role == UserRole.SalesRep)
            query = query.Where(r => r.OwnerUserId == _currentUser.UserId);

        return await query
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}
