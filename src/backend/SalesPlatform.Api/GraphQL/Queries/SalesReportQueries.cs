using HotChocolate.Authorization;
using MediatR;
using SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

namespace SalesPlatform.Api.GraphQL.Queries;

[Authorize]
public class SalesReportQueries
{
    public Task<SalesReportsPage> SalesReports(
        [Service] IMediator mediator,
        CancellationToken cancellationToken,
        string? search = null,
        string? sortBy = null,
        bool descending = true,
        int skip = 0,
        int take = 50)
        => mediator.Send(new GetSalesReportListQuery(search, sortBy, descending, skip, take), cancellationToken);
}