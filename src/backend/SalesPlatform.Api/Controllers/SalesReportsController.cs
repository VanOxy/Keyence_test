using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPlatform.Application.Common.Exceptions;
using SalesPlatform.Application.SalesReports.Commands.DeleteSalesReport;
using SalesPlatform.Application.SalesReports.Commands.ImportSalesReport;

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
            throw new ValidationException("File", "File is empty.");

        if (!System.IO.Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new ValidationException("File", "Only .xlsx files are allowed.");

        var ownerUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new ImportSalesReportCommand(file.OpenReadStream(), file.FileName, ownerUserId);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSalesReportCommand(id), cancellationToken);
        return NoContent();
    }
}
