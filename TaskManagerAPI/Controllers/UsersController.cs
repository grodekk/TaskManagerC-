using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _service.GetAllAsync();

        return Ok(users);
    }

    [HttpPut("{id}/role")]
    public async Task<IActionResult> UpdateRole(string id, UpdateUserRoleDto dto)
    {
        var result = await _service.UpdateRoleAsync(id, dto.Role);

        switch (result)
        {
            case UpdateRoleResult.Success:
                return NoContent();

            case UpdateRoleResult.UserNotFound:
                return NotFound("User not found.");

            case UpdateRoleResult.InvalidRole:
                return BadRequest("Invalid role.");

            case UpdateRoleResult.Failed:
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Could not update user role.");

            default:
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}