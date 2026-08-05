using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        return int.Parse(userIdClaim!.Value);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var userId = GetUserId();
        var projects = _service.GetAll(userId);

        return Ok(projects);
    }

    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var userId = GetUserId();
        var project = _service.GetById(id, userId);

        if (project == null)
            return NotFound();

        return Ok(project);
    }
    

    [HttpPost]
    public IActionResult Create(CreateProjectDto dto)
    {
        var userId = GetUserId();

        var project = _service.Create(dto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            project
        );
    }
}