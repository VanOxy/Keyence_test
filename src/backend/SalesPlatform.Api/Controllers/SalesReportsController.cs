using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport;
using SalesPlatform.Application.SalesReports.Queries.GetSalesReportList;

namespace SalesPlatform.Api.Controllers;

[ApiController]
[Route("api/sales-reports")]
[Authorize]
public class SalesReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ImportResult>> Import(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest("File is empty.");

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are allowed.");

        var ownerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new ImportSalesReportCommand(file.OpenReadStream(), file.FileName, ownerUserId);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalesReportListItem>>> GetList(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesReportListQuery(), cancellationToken);
        return Ok(result);
    }
}
