using MediatR;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

namespace SalesPlatform.Application.SalesRecords.Commands.UpdateSalesRecord;

public record UpdateSalesRecordCommand(
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
    decimal Revenue) : IRequest<SalesRecordListItem>;
