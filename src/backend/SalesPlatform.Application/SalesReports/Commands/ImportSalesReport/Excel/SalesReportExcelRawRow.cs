namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

// A single data row from the Excel file, prior to any typing or validation.
// Cells are read as `object` (ClosedXML specifics are encapsulated within the Infrastructure layer),
// and the dictionary key is the column header name rather than the column letter 
// (making it resilient to changes in column order).
public class SalesReportExcelRawRow
{
    public required int RowNumber { get; set; }
    public required IReadOnlyDictionary<string, object?> Cells { get; set; }
}
