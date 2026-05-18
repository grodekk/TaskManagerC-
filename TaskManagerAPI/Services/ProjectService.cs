using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public class ProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db)
    {
        _db = db;
    }

    public List<ProjectWithTasksDto> GetAll()
    {
        return _db.Projects
            .Include(p => p.Tasks)
            .Select(p => new ProjectWithTasksDto
            {
                Id = p.Id,
                Name = p.Name,
                Tasks = p.Tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsDone = t.IsDone
                }).ToList()
            })
            .ToList();
    }

    public Project Create(CreateProjectDto dto)
    {
        var project = new Project { Name = dto.Name };
        _db.Projects.Add(project);
        _db.SaveChanges();
        return project;
    }
}