using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Application.Features.Users.Commands.RegisterUser;
using SchoolPortal.Application.Features.Users.Queries.GetUserById;

namespace SchoolPortal.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class UsersController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Registers a user and attaches them to the current school with a role (Status = Invited).
    /// The tenant comes from the X-School-Id header (dev) / school_id claim; it is not in the body.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RegisterUserResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
    }

    /// <summary>Gets a user visible to the current tenant (has at least one membership here). 404 otherwise.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetUserByIdQuery(id), ct));
}
