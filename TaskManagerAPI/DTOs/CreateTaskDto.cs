namespace TaskManagerAPI.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsDone { get; set; }
}

public class CreateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}

public class UpdateTaskDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}

public class UpdateTaskStatusDto
{
    public bool IsDone { get; set; }
}