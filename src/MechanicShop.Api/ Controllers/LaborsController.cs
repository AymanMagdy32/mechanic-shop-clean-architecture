using MechanicShop.Application.Features.Labors.Query;
using MechanicShop.Application.Labors.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/labors")]
public sealed class LaborsController(ISender sender) : ControllerBase
{
    // GET: api/labors
    [HttpGet]
    [ProducesResponseType(typeof(List<LaborDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetLabors(
        CancellationToken ct)
    {
        var result = await sender.Send(
            new GetLaborsQuery(),
            ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}