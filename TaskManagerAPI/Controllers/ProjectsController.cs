using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _service;

    public ProjectsController(ProjectService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var project = _service.GetById(id);
        if (project == null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public IActionResult Create(CreateProjectDto dto)
    {
        var project = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }
}