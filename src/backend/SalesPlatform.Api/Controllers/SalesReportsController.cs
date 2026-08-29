using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesPlatform.Application.Common.Interfaces;

namespace SalesPlatform.Api.Controllers;

[ApiController]
[Route("api/sales-reports")]
[Authorize]
public class SalesReportsController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public SalesReportsController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    // tmp
    [HttpPost]
    public async Task<ActionResult> Import(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest("File is empty.");

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .xlsx files are allowed.");

        var filePath = await _fileStorage.SaveAsync(file.OpenReadStream(), file.FileName, cancellationToken);

        try
        {
            return Ok(new { file.FileName, file.Length, savedTo = filePath });
        }
        finally
        {
            _fileStorage.Delete(filePath);
        }
    }
}
