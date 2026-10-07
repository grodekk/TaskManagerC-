namespace TaskManagerAPI.DTOs;

public class UserDto
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public string? Role { get; set; }
}