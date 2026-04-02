using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private static List<TaskItem> tasks = new();

    // GET: /api/tasks
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(tasks);
    }

    // GET: /api/tasks/1
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var task = tasks.FirstOrDefault(x => x.Id == id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    // POST: /api/tasks
    [HttpPost]
    public IActionResult Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = tasks.Count + 1,
            Title = dto.Title,
            IsDone = false
        };

        tasks.Add(task);

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    // DELETE: /api/tasks/1
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var task = tasks.FirstOrDefault(x => x.Id == id);

        if (task == null)
            return NotFound();

        tasks.Remove(task);

        return NoContent();
    }
}