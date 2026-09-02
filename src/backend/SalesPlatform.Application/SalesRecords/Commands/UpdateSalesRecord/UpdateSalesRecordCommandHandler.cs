using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Exceptions;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesRecords.Commands.UpdateSalesRecord;

public class UpdateSalesRecordCommandHandler : IRequestHandler<UpdateSalesRecordCommand, SalesRecordListItem>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateSalesRecordCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<SalesRecordListItem> Handle(UpdateSalesRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _db.SalesRecords.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (record is null)
            throw new NotFoundException($"Sales record {request.Id} was not found.");

        // SalesRep can only edit their own records; Manager/Admin can edit anyone's.
        // Same message either way — we don't confirm to a SalesRep that someone else's record exists.
        if (_currentUser.Role == UserRole.SalesRep && record.OwnerUserId != _currentUser.UserId)
            throw new NotFoundException($"Sales record {request.Id} was not found.");

        record.Date = request.Date;
        record.CompanyName = request.CompanyName;
        record.ContactPerson = request.ContactPerson;
        record.Phone = request.Phone;
        record.Email = request.Email;
        record.Region = request.Region;
        record.Product = request.Product;
        record.UnitsSold = request.UnitsSold;
        record.UnitPrice = request.UnitPrice;
        record.Revenue = request.Revenue;

        await _db.SaveChangesAsync(cancellationToken);

        return new SalesRecordListItem(
            record.Id,
            record.Date,
            record.CompanyName,
            record.ContactPerson,
            record.Phone,
            record.Email,
            record.Region,
            record.Product,
            record.UnitsSold,
            record.UnitPrice,
            record.Revenue);
    }
}
