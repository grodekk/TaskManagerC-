namespace TaskManagerAPI.Models;

public class Vehicle
{
    public int Id { get; set; }

    public required string RegistrationNumber { get; set; }

    public string? Make { get; set; }
    public string? Model { get; set; }

    public List<ServiceRecord> ServiceRecords { get; set; } = new();
    public List<MaintenancePlan> MaintenancePlans { get; set; } = new();
}