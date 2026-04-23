using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs;

public class TaskDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }
}

public class CreateTaskDto
{
    [Required]
    [MaxLength(100)]
    [MinLength(1)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateTaskDto
{
    [Required]
    [MaxLength(100)]
    [MinLength(1)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateTaskStatusDto
{
    public bool IsDone { get; set; }
}