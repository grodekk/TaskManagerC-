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

    public List<ProjectWithTasksDto> GetAll(int userId)
    {
        var projects = _db.Projects
            .Include(p => p.Tasks)
            .Where(p => p.UserId == userId)
            .ToList();

        return projects.Select(p => new ProjectWithTasksDto
        {
            Id = p.Id,
            Name = p.Name,
            Tasks = p.Tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone,
                ProjectName = p.Name,
            }).ToList()
        }).ToList();
    }

    public ProjectWithTasksDto Create(CreateProjectDto dto, int userId)
    {
        var project = new Project
        {
            Name = dto.Name,
            UserId = userId
        };

        _db.Projects.Add(project);
        _db.SaveChanges();

        return new ProjectWithTasksDto
        {
            Id = project.Id,
            Name = project.Name,
            Tasks = new List<TaskDto>()
        };
    }

    public ProjectWithTasksDto? GetById(int id, int userId)
    {
        var project = _db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefault(p => p.Id == id && p.UserId == userId);

        if (project == null)
            return null;

        return new ProjectWithTasksDto
        {
            Id = project.Id,
            Name = project.Name,
            Tasks = project.Tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = project.Name,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone
            }).ToList()
        };
    }

}