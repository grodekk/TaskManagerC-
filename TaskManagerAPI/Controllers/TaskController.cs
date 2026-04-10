using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TaskService _service;

    public TasksController(TaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var task = _service.GetById(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var task = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateTaskDto dto)
    {
        var success = _service.Update(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public IActionResult UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var success = _service.UpdateStatus(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var success = _service.Delete(id);
        if (!success) return NotFound();
        return NoContent();
    }
}