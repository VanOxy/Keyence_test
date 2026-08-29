using MediatR;

namespace SalesPlatform.Application.SalesReports.Commands.ImportSalesReport;

public record ImportSalesReportCommand(Stream File, string FileName, Guid OwnerUserId) : IRequest<ImportResult>;
