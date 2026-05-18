namespace TaskManagerAPI.Models;

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<TaskItem> Tasks { get; set; } = new();
}