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

    public List<TaskItem> GetAll()
    => _db.Tasks.ToList();

    public TaskItem? GetById(int id)
        => _db.Tasks.FirstOrDefault(x => x.Id == id);

    public TaskItem Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {            
            Title = dto.Title,
            Description = dto.Description,
            IsDone = false
        };

        _db.Tasks.Add(task);
        _db.SaveChanges();        

        return task;
    }

    public bool Update(int id, UpdateTaskDto dto)
    {
        var task = GetById(id);
        if (task == null) return false;

        task.Title = dto.Title;
        task.Description = dto.Description;

        _db.SaveChanges();

        return true;
    }

    public bool UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var task = GetById(id);
        if (task == null) return false;

        task.IsDone = dto.IsDone;

        _db.SaveChanges();

        return true;
    }

    public bool Delete(int id)
    {
        var task = GetById(id);
        if (task == null) return false;

        _db.Tasks.Remove(task);
        _db.SaveChanges();

        return true;
    }
}