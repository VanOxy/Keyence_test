using MediatR;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

namespace SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordDetail;

public record GetSalesRecordDetailQuery(long Id) : IRequest<SalesRecordListItem?>;
