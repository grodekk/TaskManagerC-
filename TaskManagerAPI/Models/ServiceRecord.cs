namespace TaskManagerAPI.Models;

public class ServiceRecord
{
	public int Id { get; set; }
	
	public int VehicleId { get; set; }
	public Vehicle Vehicle { get; set; } = null!;

	public DateOnly PerformedOn { get; set; }
	public int Mileage { get; set; }
		
	public List<ServiceRecordItem> Items { get; set; } = new();
}