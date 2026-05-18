namespace TaskManagerAPI.DTOs;

public class ProjectWithTasksDto
{
	public int Id { get; set; }
	public required string Name { get; set; }

	public List<TaskDto> Tasks { get; set; } = new();
}