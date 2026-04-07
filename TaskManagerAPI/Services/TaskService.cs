using TaskManagerAPI.Models;
using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public List<TaskItem> GetAll()
        => _tasks;

    public TaskItem? GetById(int id)
        => _tasks.FirstOrDefault(x => x.Id == id);

    public TaskItem Create(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Id = _nextId++,
            Title = dto.Title,
            Description = dto.Description,
            IsDone = false
        };

        _tasks.Add(task);
        return task;
    }

    public bool Update(int id, UpdateTaskDto dto)
    {
        var task = GetById(id);
        if (task == null) return false;

        task.Title = dto.Title;
        task.Description = dto.Description;

        return true;
    }

    public bool UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var task = GetById(id);
        if (task == null) return false;

        task.IsDone = dto.IsDone;
        return true;
    }

    public bool Delete(int id)
    {
        var task = GetById(id);
        if (task == null) return false;

        _tasks.Remove(task);
        return true;
    }
}