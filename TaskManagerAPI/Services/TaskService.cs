using TaskManagerAPI.Models;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Data;

namespace TaskManagerAPI.Services;

public class TaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public List<TaskDto> GetAll(int userId)
        => _db.Tasks
            .Where(t => t.Project != null && t.Project.UserId == userId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project!.Name,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone
            })
        .ToList();

    public TaskDto? GetById(int id, int userId)
        => _db.Tasks
            .Where(t => t.Id == id && t.Project != null && t.Project.UserId == userId)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                ProjectId = t.ProjectId,
                ProjectName = t.Project!.Name,
                Title = t.Title,
                Description = t.Description,
                IsDone = t.IsDone
            })
            .FirstOrDefault();

    private TaskItem? GetEntityById(int id, int userId)
    {
        return _db.Tasks.FirstOrDefault(t =>
            t.Id == id &&
            t.Project.UserId == userId);
    }

    public TaskItem? Create(CreateTaskDto dto, int userId)
    {
        var projectExists = _db.Projects.Any(p =>
            p.Id == dto.ProjectId &&
            p.UserId == userId);

        if (!projectExists)
            return null;

        var task = new TaskItem
        {            
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            IsDone = false
        };

        _db.Tasks.Add(task);
        _db.SaveChanges();        

        return task;
    }

    public bool Update(int id, UpdateTaskDto dto, int userId)
    {
        var task = GetEntityById(id, userId);

        if (task == null)
            return false;

        task.Title = dto.Title;
        task.Description = dto.Description;

        _db.SaveChanges();

        return true;
    }

    public bool UpdateStatus(int id, UpdateTaskStatusDto dto, int userId)
    {
        var task = GetEntityById(id, userId);

        if (task == null)
            return false;

        task.IsDone = dto.IsDone;

        _db.SaveChanges();

        return true;
    }

    public bool Delete(int id, int userId)
    {
        var task = GetEntityById(id, userId);        
        
        if (task == null)
            return false;

        _db.Tasks.Remove(task);
        _db.SaveChanges();

        return true;
    }

    public List<TaskItem> GetTasksByProject(int projectId, int userId)
        => _db.Tasks
            .Where(t =>
                t.ProjectId == projectId &&
                t.Project != null &&
                t.Project.UserId == userId)
            .ToList();
}