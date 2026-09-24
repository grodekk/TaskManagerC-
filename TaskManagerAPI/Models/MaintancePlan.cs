namespace TaskManagerAPI.Models;

public class MaintenancePlan
{
    public int Id { get; set; }

    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public required string Description { get; set; }
        
    public int? TargetMileage { get; set; }
    public DateOnly? TargetDate { get; set; }
        
    public bool IsCompleted { get; set; }
}