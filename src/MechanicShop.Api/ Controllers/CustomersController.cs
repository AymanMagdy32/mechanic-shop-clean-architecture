using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
using MechanicShop.Application.Features.Customers.Commands.RemoveCustomer;
using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using MechanicShop.Application.Features.Customers.Dtos;
using MechanicShop.Application.Features.Customers.Queries.GetCustomerById;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ISender sender) : ControllerBase
{
    // GET: api/customers
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomers(
        CancellationToken ct)
    {
        var result = await sender.Send(
            new GetCustomersQuery(),
            ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // GET: api/customers/{customerId}
    [HttpGet("{customerId:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerById(
        Guid customerId,
        CancellationToken ct)
    {
        var query = new GetCustomerByIdQuery(customerId);

        var result = await sender.Send(query, ct);

        if (result.IsError)
        {
            return NotFound(result.Errors);
        }

        return Ok(result.Value);
    }


    // POST: api/customers
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Value);
    }


    // PUT: api/customers/{customerId}
    [HttpPut("{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCustomer(
        Guid customerId,
        [FromBody] UpdateCustomerCommand command,
        CancellationToken ct)
    {
        if (customerId != command.CustomerId)
        {
            return BadRequest(new
            {
                Message = "Customer ID in route does not match Customer ID in request body."
            });
        }

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }


    // DELETE: api/customers/{customerId}
    [HttpDelete("{customerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteCustomer(
        Guid customerId,
        CancellationToken ct)
    {
        var command = new RemoveCustomerCommand(customerId);

        var result = await sender.Send(command, ct);

        if (result.IsError)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}