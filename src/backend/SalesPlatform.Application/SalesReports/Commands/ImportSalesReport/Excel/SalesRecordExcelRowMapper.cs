using System.Globalization;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

// Converts one raw Excel row into a typed SalesRecord, or collects structural errors
// if a cell can't be coerced into the type its target field expects (wrong type, missing value).
// This is the only validation the import does — no business rules (email format, Revenue
// formula, etc.) are checked anywhere in the pipeline.
//
// SalesReportId is left as Guid.Empty: it isn't known until the SalesReport row is inserted.
// The command handler patches it onto every mapped record right before the bulk insert.
public static class SalesRecordExcelRowMapper
{
    public static (SalesRecord? Record, IReadOnlyList<RowError> Errors) Map(SalesReportExcelRawRow row, Guid ownerUserId)
    {
        var errors = new List<RowError>();

        var date = TryParseDate(row, errors);
        var companyName = TryParseRequiredString(row, "Company Name", errors);
        var contactPerson = TryParseRequiredString(row, "Contact Person", errors);
        var phone = TryParseRequiredString(row, "Phone", errors);
        var email = TryParseRequiredString(row, "Email", errors);
        var region = TryParseRequiredString(row, "Region", errors);
        var product = TryParseRequiredString(row, "Product", errors);
        var unitsSold = TryParseInt(row, "Units Sold", errors);
        var unitPrice = TryParseDecimal(row, "Unit Price", errors);
        var revenue = TryParseDecimal(row, "Revenue", errors);

        if (errors.Count > 0)
            return (null, errors);

        var record = new SalesRecord
        {
            SalesReportId = Guid.Empty,
            OwnerUserId = ownerUserId,
            Date = date!.Value,
            CompanyName = companyName!,
            ContactPerson = contactPerson!,
            Phone = phone!,
            Email = email!,
            Region = region!,
            Product = product!,
            UnitsSold = unitsSold!.Value,
            UnitPrice = unitPrice!.Value,
            Revenue = revenue!.Value
        };

        return (record, errors);
    }

    private static DateOnly? TryParseDate(SalesReportExcelRawRow row, List<RowError> errors)
    {
        var raw = row.Cells.GetValueOrDefault("Date");
        switch (raw)
        {
            case DateTime dt:
                return DateOnly.FromDateTime(dt);
            case string s when DateOnly.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed):
                return parsed;
            default:
                errors.Add(new RowError(row.RowNumber, "Date", raw, "Expected a date (yyyy-MM-dd)."));
                return null;
        }
    }

    private static string? TryParseRequiredString(SalesReportExcelRawRow row, string column, List<RowError> errors)
    {
        var raw = row.Cells.GetValueOrDefault(column);
        var value = raw switch
        {
            string s => s.Trim(),
            double d => d.ToString(CultureInfo.InvariantCulture),
            _ => null
        };

        if (string.IsNullOrEmpty(value))
        {
            errors.Add(new RowError(row.RowNumber, column, raw, "Required, but is missing."));
            return null;
        }

        return value;
    }

    private static int? TryParseInt(SalesReportExcelRawRow row, string column, List<RowError> errors)
    {
        var raw = row.Cells.GetValueOrDefault(column);
        switch (raw)
        {
            case double d when d == Math.Floor(d):
                return (int)d;
            case string s when int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed):
                return parsed;
            default:
                errors.Add(new RowError(row.RowNumber, column, raw, "Expected a whole number."));
                return null;
        }
    }

    private static decimal? TryParseDecimal(SalesReportExcelRawRow row, string column, List<RowError> errors)
    {
        var raw = row.Cells.GetValueOrDefault(column);
        switch (raw)
        {
            case double d:
                return (decimal)d;
            case string s when decimal.TryParse(s.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed):
                return parsed;
            default:
                errors.Add(new RowError(row.RowNumber, column, raw, "Expected a number."));
                return null;
        }
    }
}
