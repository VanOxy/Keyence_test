using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

namespace SalesPlatform.Application.Tests.SalesReports.ImportSalesReport;

public class SalesRecordExcelRowMapperTests
{
    private static readonly Guid OwnerUserId = Guid.NewGuid();

    // Mirrors a row from the real sample file (HU_Sales_Report-202607.xlsx): numbers come through
    // as double (ClosedXML's XLDataType.Number), Date as the ISO text Excel stores it as.
    private static Dictionary<string, object?> ValidCells() => new()
    {
        ["Date"] = "2026-07-01",
        ["Company Name"] = "ABC Manufacturing",
        ["Contact Person"] = "John Smith",
        ["Phone"] = "+32 470123456",
        ["Email"] = "john.smith@test.com",
        ["Region"] = "HU",
        ["Product"] = "IV4",
        ["Units Sold"] = 5.0,
        ["Unit Price"] = 850.0,
        ["Revenue"] = 4250.0
    };

    private static SalesReportExcelRawRow Row(Dictionary<string, object?> cells, int rowNumber = 2) =>
        new() { RowNumber = rowNumber, Cells = cells };

    [Fact]
    public void Map_WithValidRow_ReturnsRecordWithNoErrors()
    {
        var (record, errors) = SalesRecordExcelRowMapper.Map(Row(ValidCells()), OwnerUserId);

        Assert.Empty(errors);
        Assert.NotNull(record);
        Assert.Equal(OwnerUserId, record!.OwnerUserId);
        Assert.Equal(new DateOnly(2026, 7, 1), record.Date);
        Assert.Equal("ABC Manufacturing", record.CompanyName);
        Assert.Equal(5, record.UnitsSold);
        Assert.Equal(850m, record.UnitPrice);
        Assert.Equal(4250m, record.Revenue);
    }

    [Fact]
    public void Map_WithNativeExcelDate_ParsesCorrectly()
    {
        // Not every sales rep's file will store Date as ISO text like the sample —
        // a real Excel date cell comes through as DateTime instead.
        var cells = ValidCells();
        cells["Date"] = new DateTime(2026, 7, 1);

        var (record, errors) = SalesRecordExcelRowMapper.Map(Row(cells), OwnerUserId);

        Assert.Empty(errors);
        Assert.Equal(new DateOnly(2026, 7, 1), record!.Date);
    }

    [Theory]
    [InlineData("Units Sold", "abc", "Expected a whole number.")]
    [InlineData("Unit Price", "n/a", "Expected a number.")]
    [InlineData("Revenue", "n/a", "Expected a number.")]
    [InlineData("Date", "not-a-date", "Expected a date (yyyy-MM-dd).")]
    public void Map_WithWrongCellType_ReturnsStructuralError(string column, object badValue, string expectedMessage)
    {
        var cells = ValidCells();
        cells[column] = badValue;

        var (record, errors) = SalesRecordExcelRowMapper.Map(Row(cells), OwnerUserId);

        Assert.Null(record);
        var error = Assert.Single(errors);
        Assert.Equal(column, error.Column);
        Assert.Equal(expectedMessage, error.Message);
    }

    [Theory]
    [InlineData("Company Name")]
    [InlineData("Email")]
    [InlineData("Region")]
    public void Map_WithMissingRequiredField_ReturnsStructuralError(string column)
    {
        var cells = ValidCells();
        cells[column] = null;

        var (record, errors) = SalesRecordExcelRowMapper.Map(Row(cells), OwnerUserId);

        Assert.Null(record);
        var error = Assert.Single(errors);
        Assert.Equal(column, error.Column);
        Assert.Equal("Required, but is missing.", error.Message);
    }

    [Fact]
    public void Map_WithMultipleBadCells_ReturnsAnErrorForEachOne()
    {
        // A row can fail on more than one column at once — the mapper shouldn't stop
        // at the first problem, so the user sees every fix they need to make, not just one.
        var cells = ValidCells();
        cells["Units Sold"] = "abc";
        cells["Email"] = null;

        var (record, errors) = SalesRecordExcelRowMapper.Map(Row(cells), OwnerUserId);

        Assert.Null(record);
        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, e => e.Column == "Units Sold");
        Assert.Contains(errors, e => e.Column == "Email");
    }
}
