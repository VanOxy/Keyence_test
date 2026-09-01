using MediatR;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

public record GetSalesRecordsListQuery(
    Guid? SalesReportId,
    string? Search,
    string? SortBy,
    bool Descending,
    int Skip,
    int Take) : IRequest<SalesRecordsPage>;