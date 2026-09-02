using MediatR;
using Microsoft.EntityFrameworkCore;
using SalesPlatform.Application.Common.Exceptions;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Domain.Enums;

namespace SalesPlatform.Application.SalesReports.Commands.DeleteSalesReport;

public class DeleteSalesReportCommandHandler : IRequestHandler<DeleteSalesReportCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteSalesReportCommandHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteSalesReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _db.SalesReports.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (report is null)
            throw new NotFoundException($"Sales report {request.Id} was not found.");

        // SalesRep can only delete their own reports; Manager/Admin can delete anyone's — same policy as records.
        if (_currentUser.Role == UserRole.SalesRep && report.OwnerUserId != _currentUser.UserId)
            throw new NotFoundException($"Sales report {request.Id} was not found.");

        // SalesRecords.SalesReportId is ON DELETE CASCADE — removing the report removes its records too.
        _db.SalesReports.Remove(report);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
