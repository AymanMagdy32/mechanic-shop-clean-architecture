using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.Queries.GenerateTokens;
using MechanicShop.Application.Features.Identity.Queries.GetUserInfo;
using MechanicShop.Application.Features.Identity.Queries.RefreshTokens;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/identity")]
public sealed class IdentityController(ISender sender) : ControllerBase
{
    // POST: api/identity/login
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] GenerateTokenQuery query,
        CancellationToken ct
        )
    {
        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return Unauthorized(result.Errors);
        }

        return Ok(result.Value);
    }


    // POST: api/identity/refresh
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return Unauthorized(result.Errors);
        }

        return Ok(result.Value);
    }


    // GET: api/identity/users/{userId}
    [Authorize]
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserById(
        string userId,
        CancellationToken ct)
    {
        var query = new GetUserByIdQuery(userId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }
}