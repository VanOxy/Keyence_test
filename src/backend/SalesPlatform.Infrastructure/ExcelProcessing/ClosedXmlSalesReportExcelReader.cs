using ClosedXML.Excel;
using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

namespace SalesPlatform.Infrastructure.ExcelProcessing;

public class ClosedXmlSalesReportExcelReader : ISalesReportExcelReader
{
    public IReadOnlyList<SalesReportExcelRawRow> Read(Stream fileStream)
    {
        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);

        // Row 1 holds headers: header name -> column number. Column order in the file doesn't matter.
        var headers = new Dictionary<string, int>();
        foreach (var cell in worksheet.Row(1).CellsUsed())
        {
            var name = cell.GetString().Trim();
            if (!string.IsNullOrEmpty(name))
                headers[name] = cell.Address.ColumnNumber;
        }

        var rawRows = new List<SalesReportExcelRawRow>();
        var lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRowNumber; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var cells = new Dictionary<string, object?>();
            foreach (var (header, columnNumber) in headers)
                cells[header] = ToPlainValue(row.Cell(columnNumber).Value);

            rawRows.Add(new SalesReportExcelRawRow { RowNumber = rowNumber, Cells = cells });
        }

        return rawRows;
    }

    // Converts an XLCellValue into a plain CLR object, so the Application layer stays unaware of ClosedXML.
    private static object? ToPlainValue(XLCellValue value) => value.Type switch
    {
        XLDataType.Blank => null,
        XLDataType.Text => value.GetText(),
        XLDataType.Number => value.GetNumber(),
        XLDataType.DateTime => value.GetDateTime(),
        XLDataType.Boolean => value.GetBoolean(),
        XLDataType.TimeSpan => value.GetTimeSpan(),
        _ => value.ToString()
    };
}
