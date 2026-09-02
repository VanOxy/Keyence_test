using MediatR;

namespace SalesPlatform.Application.SalesReports.Commands.DeleteSalesReport;

public record DeleteSalesReportCommand(Guid Id) : IRequest;