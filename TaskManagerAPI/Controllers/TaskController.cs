using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

using TaskManagerAPI.Models;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskService _service;

    public TasksController(TaskService service)
    {
        _service = service;
    }

    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    [HttpGet]
    public IActionResult GetAll()
    {  
        var userId = GetUserId();

        return Ok(_service.GetAll(userId));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = GetUserId();

        var task = _service.GetById(id, userId);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var userId = GetUserId();

        var task = _service.Create(dto, userId);

        if (task == null)
            return NotFound("Project not found or does not belong to the current user.");

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateTaskDto dto)
    {
        var userId = GetUserId();

        var success = _service.Update(id, dto, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var userId = GetUserId();

        var success = _service.UpdateStatus(id, dto, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var userId = GetUserId();

        var success = _service.Delete(id, userId);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("project/{projectId}")]
    public IActionResult GetTasksByProject(int projectId)
    {
        var userId = GetUserId();

        return Ok(_service.GetTasksByProject(projectId, userId));
    }
}