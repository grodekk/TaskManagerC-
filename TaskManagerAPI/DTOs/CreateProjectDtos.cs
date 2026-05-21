using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs;

public class CreateProjectDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
}