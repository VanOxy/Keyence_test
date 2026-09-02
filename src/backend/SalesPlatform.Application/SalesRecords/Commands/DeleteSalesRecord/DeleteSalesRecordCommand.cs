using MediatR;

namespace SalesPlatform.Application.SalesRecords.Commands.DeleteSalesRecord;

public record DeleteSalesRecordCommand(long Id) : IRequest;
