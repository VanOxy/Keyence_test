using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Exceptions;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesRecords.Commands.DeleteSalesRecord;

public class DeleteSalesRecordCommandHandler : IRequestHandler<DeleteSalesRecordCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteSalesRecordCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteSalesRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _db.SalesRecords.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (record is null)
            throw new NotFoundException($"Sales record {request.Id} was not found.");

        // SalesRep can only delete their own records; Manager/Admin can delete anyone's.
        if (_currentUser.Role == UserRole.SalesRep && record.OwnerUserId != _currentUser.UserId)
            throw new NotFoundException($"Sales record {request.Id} was not found.");

        _db.SalesRecords.Remove(record);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
