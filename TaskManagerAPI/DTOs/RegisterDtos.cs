using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs;

public class RegisterDto
{
	[Required]
	[MinLength(3)]
	public required string Username { get; set; }

	[Required]
	[MinLength(8)]
	public required string Password { get; set; }
}