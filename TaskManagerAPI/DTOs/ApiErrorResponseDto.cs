namespace TaskManagerAPI.DTOs;

public class ApiErrorResponse
{
    public required string Code { get; set; }
    public required string Message { get; set; }
}