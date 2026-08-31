using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SalesRecordListItem>>> GetList(
        [FromQuery] Guid? salesReportId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSalesRecordsListQuery(salesReportId), cancellationToken);
        return Ok(result);
    }
}
