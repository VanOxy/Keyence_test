namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

public record SalesRecordListItem(
    long Id,
    DateOnly Date,
    string CompanyName,
    string ContactPerson,
    string Phone,
    string Email,
    string Region,
    string Product,
    int UnitsSold,
    decimal UnitPrice,
    decimal Revenue);
