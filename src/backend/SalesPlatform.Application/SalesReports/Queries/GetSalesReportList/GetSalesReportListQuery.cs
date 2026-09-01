using MediatR;

namespace SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

public record GetSalesReportListQuery(
    string? Search,
    string? SortBy,
    bool Descending,
    int Skip,
    int Take) : IRequest<SalesReportsPage>;
