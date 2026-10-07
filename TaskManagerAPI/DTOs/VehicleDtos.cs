using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs;

public class CreateVehicleDto
{
    [Required]
    [MaxLength(20)]
    public required string RegistrationNumber { get; set; }

    [MaxLength(20)]
    public string? Make { get; set; }

    [MaxLength(20)]
    public string? Model { get; set; }
}

public class UpdateVehicleDto
{
    [Required]
    [MaxLength(20)]
    public required string RegistrationNumber { get; set; }

    [MaxLength(20)]
    public string? Make { get; set; }

    [MaxLength(20)]
    public string? Model { get; set; }
}

public class VehicleDto
{
    public int Id { get; set; }
    public required string RegistrationNumber { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
}