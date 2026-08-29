namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

// A single structural validation failure for one row of the imported Excel file
// (wrong cell type, or a required field that's empty).
public record RowError(int RowNumber, string Column, object? Value, string Message);
