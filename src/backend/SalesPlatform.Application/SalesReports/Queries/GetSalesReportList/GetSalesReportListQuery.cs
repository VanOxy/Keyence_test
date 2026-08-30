using MediatR;

namespace SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

public record GetSalesReportListQuery : IRequest<IReadOnlyList<SalesReportListItem>>;