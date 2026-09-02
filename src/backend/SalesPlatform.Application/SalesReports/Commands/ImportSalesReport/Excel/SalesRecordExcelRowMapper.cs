using System.Globalization;
using SalesPlatform.Domain.Entities;

namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport.Excel;

public static class SalesRecordExcelRowMapper
{
    private const string ColDate = "Date";
    private const string ColCompanyName = "Company Name";
    private const string ColContactPerson = "Contact Person";
    private const string ColPhone = "Phone";
    private const string ColEmail = "Email";
    private const string ColRegion = "Region";
    private const string ColProduct = "Product";
    private const string ColUnitsSold = "Units Sold";
    private const string ColUnitPrice = "Unit Price";
    private const string ColRevenue = "Revenue";

    public static readonly string[] RequiredColumns =
    [
        ColDate, ColCompanyName, ColContactPerson, ColPhone, ColEmail,
        ColRegion, ColProduct, ColUnitsSold, ColUnitPrice, ColRevenue
    ];

    public static (SalesRecord? Record, IReadOnlyList<RowError> Errors) Map(SalesReportExcelRawRow row, Guid ownerUserId)
    {
        var errors = new List<RowError>();

        var date = TryParseDate(row, ColDate, errors);
        var companyName = TryParseRequiredString(row, ColCompanyName, errors);
        var contactPerson = TryParseRequiredString(row, ColContactPerson, errors);
        var phone = TryParseRequiredString(row, ColPhone, errors);
        var email = TryParseRequiredString(row, ColEmail, errors);
        var region = TryParseRequiredString(row, ColRegion, errors);
        var product = TryParseRequiredString(row, ColProduct, errors);
        var unitsSold = TryParseInt(row, ColUnitsSold, errors);
        var unitPrice = TryParseDecimal(row, ColUnitPrice, errors);
        var revenue = TryParseDecimal(row, ColRevenue, errors);

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

    private static DateOnly? TryParseDate(SalesReportExcelRawRow row, string column, List<RowError> errors)
    {
        var raw = row.Cells.GetValueOrDefault(column);
        switch (raw)
        {
            case DateTime dt:
                return DateOnly.FromDateTime(dt);
            case string s when DateOnly.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed):
                return parsed;
            default:
                errors.Add(new RowError(row.RowNumber, column, raw, "Expected a date (yyyy-MM-dd)."));
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