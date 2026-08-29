using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport;

// All-or-nothing: Success is true only if every row passed. On failure, nothing was written —
// SalesReportId is null and Errors lists every row that failed structural validation.
public record ImportResult(
    bool Success,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    IReadOnlyList<RowError> Errors,
    Guid? SalesReportId);
