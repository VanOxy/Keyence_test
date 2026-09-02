namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

public static class SalesReportHeaderValidator
{
    // Returns the required columns missing from the file's header row — empty means valid.
    public static IReadOnlyList<string> ValidateHeaders(IEnumerable<string> actualHeaders)
    {
        var actual = new HashSet<string>(actualHeaders, StringComparer.OrdinalIgnoreCase);
        return SalesRecordExcelRowMapper.RequiredColumns
            .Where(required => !actual.Contains(required))
            .ToArray();
    }
}