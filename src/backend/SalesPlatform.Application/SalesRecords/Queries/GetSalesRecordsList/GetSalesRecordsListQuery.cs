using MediatR;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

public record GetSalesRecordsListQuery(Guid? SalesReportId) : IRequest<IReadOnlyList<SalesRecordListItem>>;