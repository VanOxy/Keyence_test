namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

// Reads a worksheet from an Excel file: the first row contains column headers, and the subsequent rows contain data.
// It has no knowledge of the `SalesRecord` business fields; it simply returns raw cell values ​​based on the header name.
public interface ISalesReportExcelReader
{
    IReadOnlyList<SalesReportExcelRawRow> Read(Stream fileStream);
}