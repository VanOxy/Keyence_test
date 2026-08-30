using MediatR;
using SalesPlatform.Application.Common.Interfaces;
using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport;

public class ImportSalesReportCommandHandler : IRequestHandler<ImportSalesReportCommand, ImportResult>
{
    private readonly IFileStorage _fileStorage;
    private readonly ISalesReportExcelReader _excelReader;
    private readonly IAppDbContext _db;

    public ImportSalesReportCommandHandler(IFileStorage fileStorage, ISalesReportExcelReader excelReader, IAppDbContext db)
    {
        _fileStorage = fileStorage;
        _excelReader = excelReader;
        _db = db;
    }

    public async Task<ImportResult> Handle(ImportSalesReportCommand request, CancellationToken cancellationToken)
    {
        // Saves file to tmp folder
        var filePath = await _fileStorage.SaveAsync(request.File, request.FileName, cancellationToken);

        try
        {
            IReadOnlyList<SalesReportExcelRawRow> rawRows;
            await using (var readStream = File.OpenRead(filePath))
            {
                rawRows = _excelReader.Read(readStream);
            }

            var records = new List<SalesRecord>();
            var errors = new List<RowError>();
            var validRowCount = 0;

            var report = new SalesReport
            {
                Id = Guid.NewGuid(),
                OwnerUserId = request.OwnerUserId,
                OriginalFileName = request.FileName,
                CreatedAtUtc = DateTime.UtcNow
            };

            foreach (var rawRow in rawRows)
            {
                var (record, rowErrors) = SalesRecordExcelRowMapper.Map(rawRow, request.OwnerUserId);
                if (rowErrors.Count > 0)
                {
                    errors.AddRange(rowErrors);
                }
                else
                {
                    validRowCount++;

                    if (errors.Count > 0) continue;
                    
                    record!.SalesReportId = report.Id;
                    records.Add(record);
                }
            }

            // All-or-nothing: a single bad row means nothing from this file is written.
            if (errors.Count > 0)
            {
                return new ImportResult(
                    Success: false,
                    TotalRows: rawRows.Count,
                    ValidRows: validRowCount,
                    InvalidRows: errors.Count,
                    Errors: errors,
                    SalesReportId: null);
            } 

            _db.SalesReports.Add(report);
            _db.SalesRecords.AddRange(records);
            await _db.SaveChangesAsync(cancellationToken);

            return new ImportResult(
                Success: true,
                TotalRows: rawRows.Count,
                ValidRows: validRowCount,
                InvalidRows: 0,
                Errors: errors,
                SalesReportId: report.Id);
        }
        finally
        {
            _fileStorage.Delete(filePath);
        }
    }
}