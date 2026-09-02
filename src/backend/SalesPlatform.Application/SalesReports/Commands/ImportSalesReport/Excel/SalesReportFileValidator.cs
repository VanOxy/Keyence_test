namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

public static class SalesReportFileValidator
{
    // Null means the file is fine to map row-by-row. Otherwise, the ImportResult to return as-is.
    public static ImportResult? Validate(IReadOnlyList<SalesReportExcelRawRow> rawRows)
    {
        if (rawRows.Count == 0)
        {
            return Failed(0, "No data rows found. Check that the file has a header row with the expected columns " +
                              $"({string.Join(", ", SalesRecordExcelRowMapper.RequiredColumns)}) and at least one row of data below it.");
        }

        var missingColumns = SalesReportHeaderValidator.ValidateHeaders(rawRows[0].Cells.Keys);
        if (missingColumns.Count > 0)
        {
            return Failed(rawRows.Count,
                $"This file doesn't match the expected sales report format. Missing column(s): {string.Join(", ", missingColumns)}. " +
                $"Expected columns: {string.Join(", ", SalesRecordExcelRowMapper.RequiredColumns)}.");
        }

        return null;
    }

    private static ImportResult Failed(int totalRows, string message) => new(
        Success: false,
        TotalRows: totalRows,
        ValidRows: 0,
        InvalidRows: 1,
        Errors: [new RowError(0, "File", null, message)],
        SalesReportId: null);
}