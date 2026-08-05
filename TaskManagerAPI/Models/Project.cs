namespace TaskManagerAPI.Models;

public class Project
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;    

    public List<TaskItem> Tasks { get; set; } = new();
}