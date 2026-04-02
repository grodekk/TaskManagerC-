namespace TaskManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}