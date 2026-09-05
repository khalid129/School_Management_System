using MediatR;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Application.Common.Models;
using SchoolPortal.Application.Features.Students.Commands.CreateStudent;
using SchoolPortal.Application.Features.Students.Commands.UpdateStudent;
using SchoolPortal.Application.Features.Students.Queries.GetAllStudents;
using SchoolPortal.Application.Features.Students.Queries.GetStudentById;

namespace SchoolPortal.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class StudentsController(ISender sender) : ControllerBase
{
    /// <summary>Admits a new student into the current tenant.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateStudentCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Gets one student by id. 404 if it does not exist in the current tenant.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDto>> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetStudentByIdQuery(id), ct));

    /// <summary>Lists students in the current tenant, newest first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StudentListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<StudentListItemDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetAllStudentsQuery(page, pageSize, status, search), ct));

    /// <summary>Updates a student's mutable profile fields.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateStudentRequest body, CancellationToken ct)
    {
        await sender.Send(
            new UpdateStudentCommand(id, body.FirstName, body.LastName, body.Gender, body.RollNumber), ct);
        return NoContent();
    }

    /// <summary>Request body for <see cref="Update"/> (id comes from the route).</summary>
    public sealed record UpdateStudentRequest(
        string FirstName,
        string LastName,
        string? Gender = null,
        string? RollNumber = null);
}
