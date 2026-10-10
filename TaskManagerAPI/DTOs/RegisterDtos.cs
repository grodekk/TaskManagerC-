using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs;

public class RegisterDto
{
	[Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public required string Username { get; set; }

	[Required]	
	public required string Password { get; set; }
}