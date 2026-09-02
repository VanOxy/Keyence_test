using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPlatform.Application.SalesRecords.Commands.DeleteSalesRecord;
using SalesPlatform.Application.SalesRecords.Commands.UpdateSalesRecord;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordDetail;
using SalesPlatform.Application.SalesRecords.Queries.GetSalesRecordsList;

namespace SalesPlatform.Api.Controllers;

[ApiController]
[Route("api/sales-records")]
[Authorize]
public class SalesRecordsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesRecordsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SalesRecordListItem>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesRecordDetailQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SalesRecordListItem>> Update(long id, [FromBody] UpdateSalesRecordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { Id = id }, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSalesRecordCommand(id), cancellationToken);
        return NoContent();
    }
}