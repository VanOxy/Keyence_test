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

            if (rawRows.Count == 0)
            {
                var noRowsError = new RowError(0, "File", null,
                    "No data rows found. Check that the file has a header row with the expected columns " +
                    "(Date, Company Name, Contact Person, Phone, Email, Region, Product, Units Sold, Unit Price, Revenue) " +
                    "and at least one row of data below it.");

                return new ImportResult(
                    Success: false,
                    TotalRows: 0,
                    ValidRows: 0,
                    InvalidRows: 1,
                    Errors: [noRowsError],
                    SalesReportId: null);
            }

            var missingColumns = SalesReportHeaderValidator.ValidateHeaders(rawRows[0].Cells.Keys);
            if (missingColumns.Count > 0)
            {
                var formatError = new RowError(0, "File", null,
                    $"This file doesn't match the expected sales report format. Missing column(s): {string.Join(", ", missingColumns)}. " +
                    $"Expected columns: {string.Join(", ", SalesRecordExcelRowMapper.RequiredColumns)}.");

                return new ImportResult(
                    Success: false,
                    TotalRows: rawRows.Count,
                    ValidRows: 0,
                    InvalidRows: 1,
                    Errors: [formatError],
                    SalesReportId: null);
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