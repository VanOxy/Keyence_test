using HotChocolate.Authorization;
using HotChocolate.Types;
using MediatR;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

namespace SalesPlatform.Api.GraphQL.Queries;

// Extends the same Query root SalesReportQueries defines — HotChocolate only allows one type
// registered via AddQueryType<T>(), so a second class of query fields is added as an extension
// instead of a second AddQueryType<T>() call. The root type keeps SalesReportQueries' own name
// (not the generic "Query"), so the extension target has to be that concrete type, not
// OperationTypeNames.Query — the latter compiles and builds the schema fine but silently never
// merges the field in.
[ExtendObjectType(typeof(SalesReportQueries))]
[Authorize]
public class SalesRecordQueries
{
    public Task<SalesRecordsPage> SalesRecords(
        [Service] IMediator mediator,
        CancellationToken cancellationToken,
        string? search = null,
        Guid? salesReportId = null,
        string? sortBy = null,
        bool descending = true,
        int skip = 0,
        int take = 50)
        => mediator.Send(new GetSalesRecordsListQuery(salesReportId, search, sortBy, descending, skip, take), cancellationToken);
}
